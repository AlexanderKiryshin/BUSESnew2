using TMPro;
using UnityEngine;

public class TextInScene : MonoBehaviour
{
    [SerializeField] private GameChecker _gameChecker;
    [SerializeField] private TMP_Text _sceneText;

    private void OnEnable()
    {
        _gameChecker.WinGame += OnWinGame;
        _gameChecker.LoseGame += OnLoseGame;
    }

    private void OnLoseGame()
    {
        _sceneText.text = "YOU LOSE";
    }

    private void OnWinGame()
    {
        _sceneText.text = "YOU WIN!";
    }
}
