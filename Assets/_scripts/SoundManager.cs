using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private GameChecker _gameChecker;
    [SerializeField] private AudioSource _other, _musicSource, _uiSource;
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private AudioClip _buttonClip;
    [SerializeField] private AudioClip _winClip, _loseClip;
    [SerializeField] private AudioClip _busRunningClip, _takeCoinsClip, _damageClip, _clickOnBusClip, _errorClip;
    [SerializeField] private AudioClip[] personLoadClips;
    [SerializeField] private AudioMixer _audioMixer;

    public static SoundManager instance;

    private bool _isMusicOn = true;
    private bool _isSoundOn = true;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _gameChecker.LoseGame += PlayLoseSound;
        _gameChecker.WinGame += PlayWinSound;
    }

    private void OnDestroy()
    {
        _gameChecker.LoseGame -= PlayLoseSound;
        _gameChecker.WinGame -= PlayWinSound;
    }

    public void PlayButtonSound()
    {
        if (_isSoundOn)
            _uiSource.PlayOneShot(_buttonClip);
    }

    public void PlayWinSound()
    {
        _musicSource.Stop();
        if (_isSoundOn)
        {
            _other.PlayOneShot(_winClip);
        }
    }

    public void PlayLoseSound()
    {
        _musicSource.Stop();
        if (_isSoundOn)
        {
            _other.PlayOneShot(_loseClip);
        }
    }

    public void PlayClickOnBusSound()
    {
        if (_isSoundOn)
        {
            _uiSource.PlayOneShot(_clickOnBusClip);
        }
    }

    public void PlayBusDamagedSound()
    {
        if (_isSoundOn)
        {
            _other.PlayOneShot(_damageClip);
        }
    }

    public void PlayErrorSound()
    {
        if (_isSoundOn)
        {
            _uiSource.PlayOneShot(_errorClip);
        }
    }

    public void PlayBusRuningSound()
    {
        if (_isSoundOn)
        {
            _other.PlayOneShot(_busRunningClip);
        }
    }

    public void PlayPersonLoadSound()
    {
        if (_isSoundOn)
        {
            int random = Random.Range(0, personLoadClips.Length);
            _other.PlayOneShot(personLoadClips[random]);
        }
    }


    public void PlayAddCoinsSound()
    {
        if (_isSoundOn)
        {
            _other.PlayOneShot(_takeCoinsClip);
        }
    }
}