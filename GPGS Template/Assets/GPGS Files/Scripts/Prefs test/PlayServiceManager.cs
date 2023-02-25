using System;
using System.IO;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayServiceManager : MonoBehaviour
{
    #region Singleton

    public static PlayServiceManager Instance;

    [Tooltip("If True, Methods will not be called")]
    public bool editorMode;
    
    #region Variables

    /// <summary>
    /// will be executed on successful data load from cloud.
    /// </summary>
    public Action onDataLoaded;
    
    /// <summary>
    /// Will be executed if no data is found on the cloud.
    /// </summary>
    public Action noDataFound;
    
    /// <summary>
    /// Will be executed on failing to load data from cloud.
    /// </summary>
    public Action onDataLoadFailed;
    
    /// <summary>
    /// Will be executed on successful data save onto cloud.
    /// </summary>
    public Action onDataSaved;
    
    /// <summary>
    /// Will be executed on failing to save data onto cloud.
    /// </summary>
    public Action onDataSaveFailed;
    
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
    
    
    private bool mMIsSaving;

    #endregion


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
    
    private PlayGamesClientConfiguration _mClientConfiguration;

    private void Start()
    {
        if (editorMode)
        {
            PopupManager.Instance.ShowPopup("Editor mode active. Play Service Manager is inactive.", onlyLog:true);
            onSignedIn?.Invoke();
            return;
        }
        ConfigureGPGS();
        //SignIntoGPGS(SignInInteractivity.CanPromptAlways, _mClientConfiguration);
    }

    /// <summary>
    /// Configure the GPGS script.
    /// </summary>
    private void ConfigureGPGS()
    {
        _mClientConfiguration = new PlayGamesClientConfiguration.Builder()
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
                    onSignedIn?.Invoke();
                    PopupManager.Instance.ShowPopup("Successfully Authenticated", onlyLog:true);
                    PopupManager.Instance.ShowPopup("Hello " + Social.localUser.userName + " " +
                                                    "You have an ID of " + Social.localUser.id, title:"Success");
                    
                }
                else
                {
                    onSignInFailed?.Invoke();
                    PopupManager.Instance.ShowPopup("Failed to Authenticate", onlyLog:true);
                    PopupManager.Instance.ShowPopup("Failed to Authenticate, reason for failure is: " + code, onlyLog:true);
                }
            }
        );
    }

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
        SignIntoGPGS(SignInInteractivity.CanPromptAlways, _mClientConfiguration);
        Debug.Log("I was clicked");
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
        onSignedOut?.Invoke();
    }

    /// <summary>
    /// Method will be call for both save and load data from cloud. After successfully signing in into
    /// google account. You can call this method to save and load data. 
    /// </summary>
    /// <param name="saving">Set it true if your saving data. False if loading</param>
    public void OpenSave(bool saving)
    {
        if (editorMode)
        {
            PopupManager.Instance.ShowPopup("Editor mode action. Play Service Manager is inactive.", onlyLog:true);
            return;
        }
        PopupManager.Instance.ShowPopup("Open Saved Clicked", onlyLog:true);
        mMIsSaving = saving;

        if (Social.localUser.authenticated)
        {
            PopupManager.Instance.ShowPopup("User is authenticated", onlyLog:true);
            
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
                "SaveGameFileName",
                DataSource.ReadNetworkOnly,
                ConflictResolutionStrategy.UseMostRecentlySaved,
                SaveGameOpen
            );
        }
        else
        {
            onDataSaveFailed?.Invoke();
            onDataLoadFailed?.Invoke();
        }
    }
    
    private void SaveGameOpen(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            PopupManager.Instance.ShowPopup("Status successful.", onlyLog:true);
            if (mMIsSaving) // saving data to cloud
            {
                PopupManager.Instance.ShowPopup("Attempting to save...", onlyLog:true);
                
                // var storage = FileHandler.Load();
                if (!FileHandler.FileExists())
                {
                    PopupManager.Instance.ShowPopup("No local data found.");
                    return;
                }
                // Todo:  Load game data from local storage
                var storage = FileHandler.Load();
        
                fsData serializedData;
                var serializer = new fsSerializer();
                serializer.TrySerialize(storage, out serializedData).AssertSuccessWithoutWarnings();
                var json = fsJsonPrinter.PrettyJson(serializedData);
                
                // todo: convert datatype to byte array
                var myData = System.Text.Encoding.ASCII.GetBytes(json);
                
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
            PopupManager.Instance.ShowPopup("Status unsuccessful, failed to open save data.");
        }
    }


    private void LoadCallBack(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            PopupManager.Instance.ShowPopup("Load successful...", onlyLog:true); 
            var loadedData = System.Text.Encoding.ASCII.GetString(data);
            //Todo: Save game data back to local storage.
            if (loadedData is "" or null)
            {
                noDataFound?.Invoke();
                PopupManager.Instance.ShowPopup("No data found on the cloud.", onlyLog:true);
            }
            else
            {
                
                var converted = fsJsonParser.Parse(loadedData);
                object deserialized = null;
                var serializer = new fsSerializer();
                serializer.TryDeserialize(converted, typeof(GameDataClass), ref deserialized).AssertSuccessWithoutWarnings();

                var storage = deserialized as GameDataClass;
                FileHandler.Save(storage);
                
                
                // var file = new StreamWriter(FileHandler.FileName);
                //
                // file.WriteLine(loadedData);
                // file.Close();
                
                PopupManager.Instance.ShowPopup("Data downloaded from cloud and saved to disk.", onlyLog:true);
                onDataLoaded?.Invoke();
            }
            
        }
        else
        {
            PopupManager.Instance.ShowPopup("Failed to load data.", onlyLog:true);
            onDataLoadFailed?.Invoke();
        }
    }

    private void SaveCallBack(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            PopupManager.Instance.ShowPopup("Successfully saved to the cloud.", "Success");
            onDataSaved?.Invoke();
        }
        else
        {
            PopupManager.Instance.ShowPopup("Failed to save to cloud", "Failed", onlyLog:true);
            onDataSaveFailed?.Invoke();
        }
    }
}
