using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayServiceManager : MonoBehaviour
{
    #region Singleton

    public static PlayServiceManager Instance;

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
                    PopupManager.Instance.ShowPopup("Failed to Authenticate", onlyLog:true);
                    PopupManager.Instance.ShowPopup("Failed to Authenticate, reason for failure is: " + code, onlyLog:true);
                }
            }
        );
    }

    #endregion

    #region Variables

    public Action dataLoaded;
    public Action dataLoadFailed;
    public Action dataSaved;
    public Action dataSaveFailed;
    public Action onSignedIn;
    public Action onSignedOut;
    private string mDataToBeSaved; 
    public string loadedData;
    private bool mIsSaving;

    #endregion
    
    /// <summary>
    /// Manual Signin triggered by pressing Btn by user. 
    /// </summary>
    public void BasicSignInBtn()
    {
        SignIntoGPGS(SignInInteractivity.CanPromptAlways, mClientConfiguration);
    }
    
    /// <summary>
    /// Sign-out from account. 
    /// </summary>
    public void SignOutBtn()
    {
        PlayGamesPlatform.Instance.SignOut();
        PopupManager.Instance.ShowPopup("Signed Out", "Success");
        onSignedOut!.Invoke();
    }

    /// <summary>
    /// Method will be call for both save and load data from cloud. After successfully signing in into
    /// google account. You can call this method to save or load data. 
    /// </summary>
    /// <param name="saving">Set it true if your saving data. False if loading</param>
    /// <param name="json">If saving data then insert the data as json string. On loading leave it.</param>
    public void OpenSave(bool saving, string json = "")
    {
        PopupManager.Instance.ShowPopup("Open Saved Clicked", onlyLog:true);
        mDataToBeSaved = json;
        mIsSaving = saving;
        
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
            if (mIsSaving) // saving data
            {
                PopupManager.Instance.ShowPopup("Attempting to save...", onlyLog:true);
                // convert datatype to byte array
                byte[] myData = System.Text.Encoding.ASCII.GetBytes(mDataToBeSaved);
                
                // update metadata 
                var updateForMetadata = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("I have updated my game at: " + DateTime.Now).Build();
                
                // commit save game
                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, updateForMetadata, myData, SaveCallBack);
            }
            else // loading data
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
            loadedData = System.Text.Encoding.ASCII.GetString(data);
            dataLoaded!.Invoke();
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
}
