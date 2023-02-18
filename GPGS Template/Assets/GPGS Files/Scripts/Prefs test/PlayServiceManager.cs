using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

public class PlayServiceManager : MonoBehaviour
{
    #region Singleton

    public static PlayServiceManager Instance;

    [Tooltip("If True, Methods will not be called")]
    [SerializeField] private bool editorMode;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region GPGS Configuration Area
    
    private PlayGamesClientConfiguration mClientConfiguration;

    private void Start()
    {
        if (editorMode)
        {
            PopupManager.Instance.ShowPopup("Editor mode active. Play Service Manager is inactive.", onlyLog:true);
            return;
        }
        ConfigureGPGS();
        SignIntoGPGS(SignInInteractivity.CanPromptOnce, mClientConfiguration);
    }

    /// <summary>
    /// Configure the GPGS script.
    /// </summary>
    private void ConfigureGPGS()
    {
        mClientConfiguration = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();
    }
    
    /// <summary>
    /// Prompt the user to signin at the start of the game.
    /// </summary>
    /// <param name="interactivity">Option on how to handle the signin process.(CanPromptOnce, CanPromptAlways, NoPrompt)
    /// </param>
    /// <param name="configuration"></param>
    private void SignIntoGPGS(SignInInteractivity interactivity, PlayGamesClientConfiguration configuration)
    {
        PlayGamesPlatform.InitializeInstance(configuration);
        PlayGamesPlatform.Activate();
        
        PlayGamesPlatform.Instance.Authenticate(interactivity, (code) =>
            {
                PopupManager.Instance.ShowPopup("Authenticating...", onlyLog:true);
                if (code == SignInStatus.Success)
                {
                    onSignedIn!.Invoke();
                    PopupManager.Instance.ShowPopup("Successfully Authenticated", onlyLog:true);
                    PopupManager.Instance.ShowPopup("Hello " + Social.localUser.userName + " " +
                                                    "You have an ID of " + Social.localUser.id, title:"Success");
                    
                }
                else
                {
                    onSignInFailed!.Invoke();
                    PopupManager.Instance.ShowPopup("Failed to Authenticate", onlyLog:true);
                    PopupManager.Instance.ShowPopup("Failed to Authenticate, reason for failure is: " + code, onlyLog:true);
                }
            }
        );
    }

    #endregion

    #region Variables

    /// <summary>
    /// will be executed on successful data load from cloud.
    /// </summary>
    public Action dataLoaded;
    
    /// <summary>
    /// Will be executed if no data is found on the cloud.
    /// </summary>
    public Action noDataFound;
    
    /// <summary>
    /// Will be executed on failing to load data from cloud.
    /// </summary>
    public Action dataLoadFailed;
    
    /// <summary>
    /// Will be executed on successful data save onto cloud.
    /// </summary>
    public Action dataSaved;
    
    /// <summary>
    /// Will be executed on failing to save data onto cloud.
    /// </summary>
    public Action dataSaveFailed;
    
    /// <summary>
    /// Will be executed on successful signIn.
    /// </summary>
    public Action onSignedIn;
    
    /// <summary>
    /// Will be executed on signIn failed.
    /// </summary>
    public Action onSignInFailed;
    
    /// <summary>
    /// Will be executed on signOut.
    /// </summary>
    public Action onSignedOut;
    
    
    private bool _mIsSaving;
    
    private string _fileName = "";
    private string _password = "";
    
    #endregion
    
    /// <summary>
    /// Manual Signin triggered by pressing Btn by user. 
    /// </summary>
    public void BasicSignInBtn()
    {
        if (editorMode)
        {
            PopupManager.Instance.ShowPopup("Editor mode active. Play Service Manager is inactive.", onlyLog:true);
            return;
        }
        SignIntoGPGS(SignInInteractivity.CanPromptAlways, mClientConfiguration);
    }
    
    /// <summary>
    /// Sign-out from account. 
    /// </summary>
    public void SignOutBtn()
    {
        if (editorMode)
            return;
        PlayGamesPlatform.Instance.SignOut();
        PopupManager.Instance.ShowPopup("Signed Out", "Success");
        onSignedOut!.Invoke();
    }

    /// <summary>
    /// Method will be call for both save and load data from cloud. After successfully signing in into
    /// google account. You can call this method to save or load data. 
    /// </summary>
    /// <param name="saving">Set it true if your saving data. False if loading</param>
    /// <param name="fileName">Name of the file in the system, that needs to be saved on the cloud</param>
    /// <param name="password">Give password if the file is encrypted. leave blank otherwise.</param>
    public void OpenSave(bool saving, string fileName = "gamedata", string password = "")
    {
        if (editorMode)
        {
            PopupManager.Instance.ShowPopup("Editor mode action. Play Service Manager is inactive.", onlyLog:true);
            return;
        }
        PopupManager.Instance.ShowPopup("Open Saved Clicked", onlyLog:true);
        _mIsSaving = saving;
        
        // Initialize the file name with the right path, name and extension.
        _fileName = Application.persistentDataPath + "/" + fileName + ".dat";
        _password = password;
        
        if (Social.localUser.authenticated)
        {
            PopupManager.Instance.ShowPopup("User is authenticated", onlyLog:true);
            
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
                "SaveGameFileName",
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                SaveGameOpen
            );
        }
    }
    
    private void SaveGameOpen(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            PopupManager.Instance.ShowPopup("Status successful.", onlyLog:true);
            if (_mIsSaving) // saving data to cloud
            {
                PopupManager.Instance.ShowPopup("Attempting to save...", onlyLog:true);
                
                // Load game data from local storage
                LoadGameData(out var gameData);
                
                // convert datatype to byte array
                var myData = System.Text.Encoding.ASCII.GetBytes(gameData);
                
                // update metadata 
                var updateForMetadata = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("I have updated my game at: " + DateTime.Now).Build();
                
                // commit save game
                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, updateForMetadata, myData, SaveCallBack);
            }
            else // loading data from cloud.
            {
                PopupManager.Instance.ShowPopup("Attempting to load...", onlyLog:true);
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, LoadCallBack);
            }
        }
        else // SavedGameRequestStatus error
        {
            PopupManager.Instance.ShowPopup("Status unsuccessful, failed to open save data.", onlyLog:true);
        }
    }



    private void LoadCallBack(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            PopupManager.Instance.ShowPopup("Load successful, attempting to print...", onlyLog:true); 
            var loadedData = System.Text.Encoding.ASCII.GetString(data);
            
            // Save game data back to local storage.
            SaveGameData(loadedData);
            
            // Fahim|25
        }
        else
        {
            PopupManager.Instance.ShowPopup("Failed to load data.", onlyLog:true);
            dataLoadFailed!.Invoke();
        }
    }

    private void SaveCallBack(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            PopupManager.Instance.ShowPopup("Successfully saved to the cloud.", "Success");
            dataSaved!.Invoke();
        }
        else
        {
            PopupManager.Instance.ShowPopup("Failed to save to cloud", "Failed", onlyLog:true);
            dataSaveFailed!.Invoke();
        }
    }
    
    
    #region Game data read/write from system

    /// <summary>
    /// Checks if game data file exists in the system.
    /// </summary>
    /// <returns>True if data exists. False otherwise.</returns>
    private bool FileExists()
    {
        return File.Exists(_fileName);
    }

    /// <summary>
    /// Load game data that needs to be saved on the cloud from system.
    /// </summary>
    /// <param name="gameData">Returns game data as Json string</param>
    /// <returns>True if successfully loaded, false otherwise.</returns>
    private bool LoadGameData(out string gameData)
    {
        // If the file doesn't exist, the method just returns false. A warning message is written into the Error property.
        if (!FileExists())
        {
            PopupManager.Instance.ShowPopup("Status unsuccessful, No game data found to be saved.", onlyLog:true);
            gameData = null;
            return false;
        }

        try
        {
            if (_password == "")
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream loadFile = File.Open(_fileName, FileMode.Open);
                var storage = (Dictionary<string, object>)bf.Deserialize(loadFile);
                loadFile.Close();
                
                gameData = JsonUtility.ToJson(storage);
                return true;
            }
            else
            {
                LoadSecure(_password,out var storage);
                gameData = JsonUtility.ToJson(storage);
                return true;
            }

        }
        catch (Exception e)
        {
            PopupManager.Instance.ShowPopup("This system exception has been thrown during loading: ", onlyLog:true);
            gameData = null;
            return false;
        }

    }
    
    /// <summary>
    /// Load a password protected and compressed file data into the 'system internal storage'
    /// and return TRUE if the loading has been completed. Return FALSE if something has gone wrong.
    /// </summary>
    /// <param name="password">Password of the file.</param>
    /// <param name="storage">Data of the file.</param>
    /// <returns></returns>
    private bool LoadSecure(string password, out Dictionary<string, object> storage)
    {
        // *****************************************************************************
        // Don't change the line below
        password = (password + "easyfilesavesecure1234").Substring(0, 16);
        // *****************************************************************************

        try
        {
            byte[] key = Encoding.UTF8.GetBytes(password);
            byte[] iv = Encoding.UTF8.GetBytes(password);

            using (Stream s = File.OpenRead(_fileName))
            {
                RijndaelManaged rm = new RijndaelManaged();
                rm.Key = key;
                rm.IV = iv;
                using (CryptoStream cs = new CryptoStream(s, rm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (GZipStream gs = new GZipStream(cs, CompressionMode.Decompress))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        storage = (Dictionary<string, object>)bf.Deserialize(gs);
                    }
                }
            }

            return true;
        }
        catch (System.Exception e)
        {
            PopupManager.Instance.ShowPopup("This system exception has been thrown during secure loading: ", onlyLog:true);
            storage = null;
            return false;
        }
    }

    /// <summary>
    /// Save the game data back to the system.
    /// </summary>
    /// <param name="dataString">Json data string loaded from cloud.</param>
    /// <returns></returns>
    private bool SaveGameData(string dataString)
    {
        try
        {
            if (_password == "")
            {
                // Converts the game data json string back to dictionary format.
                var storage = JsonUtility.FromJson<Dictionary<string, object>>(dataString);
                
                // Save the game data to system.
                BinaryFormatter bf = new BinaryFormatter();
                FileStream saveFile = File.Create(_fileName);
                bf.Serialize(saveFile, storage);
                saveFile.Close();
                
                dataLoaded!.Invoke();
                return true;
            }
            else
            {
                SaveSecure(_password, dataString);
                return true;
            }
        }
        catch (Exception e)
        {
            PopupManager.Instance.ShowPopup("This system exception has been thrown during saving: ", onlyLog:true);
            noDataFound!.Invoke();
            return false;
        }
            
    }
    
    private bool SaveSecure(string password, string dataString)
    {
        // *****************************************************************************
        // Don't change the line below
        password = (password + "easyfilesavesecure1234").Substring(0, 16);
        // *****************************************************************************

        try
        {
            // Converts the game data json string back to dictionary format.
            var storage = JsonUtility.FromJson<Dictionary<string, object>>(dataString);

            byte[] key = Encoding.UTF8.GetBytes(password);
            byte[] iv = Encoding.UTF8.GetBytes(password);

            using (Stream s = File.Create(_fileName))
            {
                RijndaelManaged rm = new RijndaelManaged();
                rm.Key = key;
                rm.IV = iv;
                using (CryptoStream cs = new CryptoStream(s, rm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (GZipStream gs = new GZipStream(cs, CompressionMode.Compress))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(gs, storage);
                        
                        dataLoaded!.Invoke();
                    }
                }
            }
            
            return true;
        }
        catch (System.Exception e)
        {
            PopupManager.Instance.ShowPopup("This system exception has been thrown during SaveSecure: ", onlyLog:true);
            return false;
        }
    }

    #endregion

}
