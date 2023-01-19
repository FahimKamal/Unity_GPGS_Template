using GooglePlayGames;
using TMPro;
using UnityEngine;

public class Achievement : MonoBehaviour
{
    public TextMeshProUGUI logTxt;

    public void ShowAchievementsUI()
    {
        Social.ShowAchievementsUI();
    }

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

    public void ListAchievements()
    {
        Social.LoadAchievements(achievements =>
        {
            logTxt.text = "Loaded Achievements" + achievements.Length;
            foreach (var ach in achievements)
            {
                logTxt.text += "/n" + " " + ach.completed;
            }
        });
    }
    
    public void ListDescriptions()
    {
        Social.LoadAchievementDescriptions(achievements =>
        {
            logTxt.text = "Loaded Achievements" + achievements.Length;
            foreach (var ach in achievements)
            {
                logTxt.text += "/n" + " " + ach.title;
            }
        });
    }

    public void GrantAchievementBtn()
    {
        DoGrantAchievement(GPGSIds.achievement_unlock_achievement);
    }
    
    public void GrantIncrementalBtn()
    {
        DoIncrementalAchievement(GPGSIds.achievement_incremental_achievement);
    }

    public void RevealAchievementBtn()
    {
        DoRevealAchievement(GPGSIds.achievement_hidden_unlock_achievement);
    }
    
    public void RevealIncrementalAchievementBtn()
    {
        DoRevealAchievement(GPGSIds.achievement_hidden_incremental_achievement);
    }

    public void GrantHiddenAchievementBtn()
    {
        DoGrantAchievement(GPGSIds.achievement_hidden_unlock_achievement);
    }

    public void HiddenIncrementalAchievementBtn()
    {
        DoIncrementalAchievement(GPGSIds.achievement_hidden_incremental_achievement);
    }
    
    
    
}
