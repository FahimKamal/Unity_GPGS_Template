using GooglePlayGames;
using TMPro;
using UnityEngine;

public class Achievement : MonoBehaviour
{
    public TextMeshProUGUI logTxt;

    /// <summary>
    /// Show achievements UI list from play services.
    /// </summary>
    public void ShowAchievementsUI()
    {
        Social.ShowAchievementsUI();
    }

    /// <summary>
    /// Grant a Achievement to the player. (Reveal ro hidden)
    /// </summary>
    /// <param name="achievement">Achievement ID. Achievement can be of reveal or hidden type.</param>
    private void DoGrantAchievement(string achievement)
    {
        Social.ReportProgress(achievement, 
            100.0f,
            (bool success) =>
            {
                if (success)
                {
                    logTxt.text = achievement + ": " + success.ToString();
                    // Perform new actions here on success.
                }
                else
                {
                    logTxt.text = achievement + ": " + success.ToString();
                    // Perform new actions here on failure. 
                }
            });
    }

    /// <summary>
    /// Reveal hidden achievement to player. Achievement can be both incremental ro non-incremental.
    /// </summary>
    /// <param name="achievement">Achievement ID</param>
    private void DoRevealAchievement(string achievement)
    {
        Social.ReportProgress(achievement, 
            0.0f,
            (bool success) =>
            {
                if (success)
                {
                    logTxt.text = achievement + ": " + success.ToString();
                    // Perform new actions here on success.
                }
                else
                {
                    logTxt.text = achievement + ": " + success.ToString();
                    // Perform new actions here on failure. 
                }
            });
    }

    /// <summary>
    /// Grant the incremental achievement to the player. Can be both reveal or hidden type.
    /// </summary>
    /// <param name="achievement">Achievement ID</param>
    private void DoIncrementalAchievement(string achievement)
    {
        PlayGamesPlatform platform = (PlayGamesPlatform)Social.Active;
        
        platform.IncrementAchievement(achievement, 
            1,
            (bool success) =>
            {
                if (success)
                {
                    logTxt.text = achievement + ": " + success.ToString();
                    // Perform new actions here on success.
                }
                else
                {
                    logTxt.text = achievement + ": " + success.ToString();
                    // Perform new actions here on failure. 
                }
            });
    }

    /// <summary>
    /// List the achievements of the game and state of them. Can be modified to do lot more.
    /// </summary>
    public void ListAchievements()
    {
        Social.LoadAchievements(achievements =>
        {
            logTxt.text = "Loaded Achievements" + achievements.Length;
            foreach (var ach in achievements)
            {
                logTxt.text += "\n" + " " + ach.completed;
            }
        });
    }
    
    /// <summary>
    /// List descriptions of all achievements in the game. Can be modified to do lot more.
    /// </summary>
    public void ListDescriptions()
    {
        Social.LoadAchievementDescriptions(achievements =>
        {
            logTxt.text = "Loaded Achievements" + achievements.Length;
            foreach (var ach in achievements)
            {
                logTxt.text += "\n" + " " + ach.title;
            }
        });
    }

    public void GrantAchievementBtn()
    {
        DoGrantAchievement(GPGSIds.ACHIEVEMENT_UNLOCK_ACHIEVEMENT);
    }
    
    public void GrantIncrementalBtn()
    {
        DoIncrementalAchievement(GPGSIds.ACHIEVEMENT_INCREMENTAL_ACHIEVEMENT);
    }

    public void RevealAchievementBtn()
    {
        DoRevealAchievement(GPGSIds.ACHIEVEMENT_HIDDEN_UNLOCK_ACHIEVEMENT);
    }
    
    public void RevealIncrementalAchievementBtn()
    {
        DoRevealAchievement(GPGSIds.ACHIEVEMENT_HIDDEN_INCREMENTAL_ACHIEVEMENT);
    }

    public void GrantHiddenAchievementBtn()
    {
        DoGrantAchievement(GPGSIds.ACHIEVEMENT_HIDDEN_UNLOCK_ACHIEVEMENT);
    }

    public void HiddenIncrementalAchievementBtn()
    {
        DoIncrementalAchievement(GPGSIds.ACHIEVEMENT_HIDDEN_INCREMENTAL_ACHIEVEMENT);
    }
    
    
    
}
