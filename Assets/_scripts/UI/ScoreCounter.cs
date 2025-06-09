using Assets._scripts;
using MirraGames.SDK;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    int score = 0;
    void Start()
    {        
        Bus.onScoreChanged += UpdateScore;
        if (MirraSDK.Data.HasKey("score"))
        { 
            scoreText.text = LocalizationManager.Instance.GetText("score") + MirraSDK.Data.GetInt("score");
            score= MirraSDK.Data.GetInt("score");
        }
        else
        {
            scoreText.text = LocalizationManager.Instance.GetText("score") + "0";
        }
    }

    public void UpdateScore(int score)
    {
        this.score += score;
        scoreText.text = LocalizationManager.Instance.GetText("score") + this.score.ToString();
        MirraSDK.Data.SetInt("score",this.score);
    }    
}
