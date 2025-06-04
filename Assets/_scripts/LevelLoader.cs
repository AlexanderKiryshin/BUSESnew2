using MirraGames.SDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private Button _nextLevelButton;
    
    private int _level;
    
    public int Level
    {
        get => _level;
        private set => _level = value;
    }

    private void Awake()
    {
        _uiManager.OnCompleteBtnClicked += LoadNextLevel;
        LoadLevelData();

        _nextLevelButton.onClick.AddListener(LoadNextLevel);
    }


    private void OnDisable()
    {
        _uiManager.OnCompleteBtnClicked -= LoadNextLevel;
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1;
        /*if (_level >= 3)
        {
            AdManager.instance.ShowInterstitial();
        }*/
        SaveLevelsData();
        Analytic.LevelStarted(_level.ToString());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void LoadLevelData()
    {
        if (MirraSDK.Data.HasKey("Level"))
        {
            _level = MirraSDK.Data.GetInt("Level");
        }
        else
        {
            _level = 0;
        }
    }

    private void CheckDifficulty()
    {
        if (_level >= 5)
        {
            MirraSDK.Data.SetInt("Difficulty", 1);
        }
    }
    
    private void SaveLevelsData()
    {
        _level++;
        CheckDifficulty();
        MirraSDK.Data.SetInt("Level", _level);
        MirraSDK.Data.Save();
    }
}