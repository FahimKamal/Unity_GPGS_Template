using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Leaderboard : MonoBehaviour
{
    public TextMeshProUGUI logTxt;
    public TMP_InputField scoreInputField;

    /// <summary>
    /// Shows leaderboardUI from Google play Services.
    /// </summary>
    public void ShowLeaderboardUI()
    {
        Social.ShowLeaderboardUI();
    }

    /// <summary>
    /// Post score to google play leaderboard.
    /// </summary>
    /// <param name="score">Score of the game.</param>
    private void DoLeaderboardPost(int score)
    {
        Social.ReportScore(score, 
            GPGSIds.LEADERBOARD_TEST_SCORE_LEADERBOARD,
            (bool success) =>
            {
                if (success)
                {
                    logTxt.text = "Score Posted of: " + score;
                    // Perform new actions here on success.
                }
                else
                {
                    logTxt.text = "Score FAiled to Post.";
                    // Perform new actions here on failure. 
                }
            });
    }

    private void LoadLeaderboard(string leaderboardID)
    {
        
        Social.LoadScores(
        leaderboardID,
        (scores) =>
        {
            if (scores.Length > 0)
            {
                logTxt.text = "Leaderboard: \n";
                foreach (var score in scores)
                {
                    logTxt.text += score.userID + ": " + score.value + "\n";
                }
                
            }
        });
    }

    public void LoadLeaderboardBtn()
    {
        LoadLeaderboard(GPGSIds.LEADERBOARD_TEST_SCORE_LEADERBOARD);
    }

    public void LeaderboardPostBtn()
    {
        DoLeaderboardPost(int.Parse(scoreInputField.text));
    }
}

public class LeaderboardData
{
    public string UserID = "";
    public string UserName = "";
    public string Score = "";
    public string Date = "";
    public string Rank = "";

    public LeaderboardData(string userID = null, string userName = null, string score = null, string date = null, string rank = null)
    {
        UserID = userID;
        UserName = userName;
        Score = score;
        Date = date;
        Rank = rank;
    }
}
