using System;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using TMPro;
using Debug = UnityEngine.Debug;

public class GPGSManager : MonoBehaviour
{
    
    
    private PlayGamesClientConfiguration mClientConfiguration;
    public TextMeshProUGUI statusTxt;
    public TextMeshProUGUI descriptionTxt;

    public GameObject homeBtn;

    public SavedGamesUI savedGamesUI;

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
                statusTxt.text = "Authenticating...";
                if (code == SignInStatus.Success)
                {
                    statusTxt.text = "Successfully Authenticated";
                    descriptionTxt.text = "Hello " + Social.localUser.userName + " " +
                                          "You have an ID of " + Social.localUser.id;
                    
                    // Activate the home Button
                    homeBtn.SetActive(true);
                }
                else
                {
                    statusTxt.text = "Failed to Authenticate";
                    descriptionTxt.text = "Failed to Authenticate, reason for failure is: " + code;
                }
            }
        );
    }

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
        statusTxt.text = "Signed Out";
        descriptionTxt.text = "";
        homeBtn.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #region SavedGames

    private bool mIsSaving;

    public void OpenSave(bool saving)
    {
        savedGamesUI.PrintLog("Open Saved Clicked");
        
        if (Social.localUser.authenticated)
        {
            savedGamesUI.PrintLog("User is authenticated");
            
            mIsSaving = saving;
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
            savedGamesUI.PrintLog("Status successful.");
            if (mIsSaving) // saving data
            {
                savedGamesUI.PrintLog("Attempting to save...");
                // convert datatype to byte array
                byte[] myData = System.Text.Encoding.ASCII.GetBytes(GetSaveString());
                
                // update metadata 
                var updateForMetadata = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("I have updated my game at: " + DateTime.Now).Build();
                
                // commit save game
                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(meta, updateForMetadata, myData, SaveCallBack);
            }
            else // loading data
            {
                savedGamesUI.PrintLog("Attempting to load...");
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, LoadCallBack);
            }
        }
        else
        {
            savedGamesUI.PrintLog("Status unsuccessful, failed to open save data.");
        }
    }

    private void LoadCallBack(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            savedGamesUI.PrintLog("Load successful, attempting to print...");
            var loadedData = System.Text.Encoding.ASCII.GetString(data);
            // Fahim|25
            LoadSavedString(loadedData);
        }
    }

    private void LoadSavedString(string cloudData)
    {
        var cloudStringArr = cloudData.Split("|");

        savedGamesUI.playerName = cloudStringArr[0];
        savedGamesUI.age = int.Parse(cloudStringArr[1]);
        
        savedGamesUI.PrintOutput();
    }

    private string GetSaveString()
    {
        var dataToSave = "";

        dataToSave += savedGamesUI.playerName;
        dataToSave += "|";
        dataToSave += savedGamesUI.age;
        
        // Fahim|25
        return dataToSave;
    }

    private void SaveCallBack(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            savedGamesUI.PrintLog("Successfully saved to the cloud.");
            Debug.Log("Successfully saved to the cloud.");
        }
        else
        {
            savedGamesUI.PrintLog("Failed to save to cloud");
            Debug.Log("Failed to save to cloud");
        }
    }

    #endregion
}
