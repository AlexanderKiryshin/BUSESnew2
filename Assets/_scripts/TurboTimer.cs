using TMPro;
using UnityEngine;

public class TurboTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _nameBtnText;
    [SerializeField] private float _duration = 60f;
    
    public bool IsTimerActive = false;
    private float _currentTime;

    void Update()
    {
        if (IsTimerActive)
        {
            _currentTime -= Time.deltaTime/2;

            if (_currentTime <= 0)
            {
                _currentTime = 0;
                StopTimer();
            }
            _timerText.text = FormatTime(_currentTime);
        }
    }

    public void StartTurbo()
    {
        _nameBtnText.enabled = false;
        _timerText.enabled = true;
        _currentTime = _duration;
        IsTimerActive = true;
        Time.timeScale = 2f; 
    }

    public void StopTimer()
    {
        _nameBtnText.enabled = true;
        _timerText.enabled = false;
        IsTimerActive = false;
        Time.timeScale = 1f; 
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
