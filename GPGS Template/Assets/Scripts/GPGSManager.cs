using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;

public class GPGSManager : MonoBehaviour
{
    private PlayGamesClientConfiguration _clientConfiguration;
    public TextMeshProUGUI statusTxt;
    public TextMeshProUGUI descriptionTxt;

    public GameObject homeBtn;

    private void Start()
    {
        ConfigureGPGS();
        SignIntoGPGS(SignInInteractivity.CanPromptOnce, _clientConfiguration);
    }

    /// <summary>
    /// Configure the GPGS script.
    /// </summary>
    internal void ConfigureGPGS()
    {
        _clientConfiguration = new PlayGamesClientConfiguration.Builder().Build();
    }

    /// <summary>
    /// Prompt the user to signin at the start of the game.
    /// </summary>
    /// <param name="interactivity">Option on how to handle the signin process.(CanPromptOnce, CanPromptAlways, NoPrompt)
    /// </param>
    /// <param name="configuration"></param>
    internal void SignIntoGPGS(SignInInteractivity interactivity, PlayGamesClientConfiguration configuration)
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
    /// Manual Signin triggered by pressing Btn user. 
    /// </summary>
    public void BasicSignInBtn()
    {
        SignIntoGPGS(SignInInteractivity.CanPromptAlways, _clientConfiguration);
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
}
