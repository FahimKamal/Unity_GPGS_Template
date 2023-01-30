using TMPro;
using UnityEngine;

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
            GPGSIds.leaderboard_test_score_leaderboard,
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
        LoadLeaderboard(GPGSIds.leaderboard_test_score_leaderboard);
    }

    public void LeaderboardPostBtn()
    {
        DoLeaderboardPost(int.Parse(scoreInputField.text));
    }
}
