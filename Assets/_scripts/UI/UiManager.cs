using System;
using System.Collections;
using _scripts.UI;
using Assets._scripts;
using MirraGames.SDK;
using TMPro;
using MirraGames.SDK.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private BusGenerator _busGenerator;
    [SerializeField] private FollowPath _followPath;
    [SerializeField] private HeliSystem _heliSystem;
    [SerializeField] private TurboTimer _turboTimer;
    [SerializeField] private ParkingManager _parkingManager;
    [SerializeField] private GameChecker _gameChecker;
    [SerializeField] private RectTransform _failed, _continue, _completed, _settings;
    [SerializeField] private CoinManager _coinManager;
    [SerializeField] private TMP_Text _coinText, _levelText, _levelTextComplete;
    [SerializeField] private GameObject _adsNotAvailible;
    [SerializeField] private GameObject _vipSlotNotEmpty;
    [SerializeField] private GameObject _noMoney;
    [SerializeField] private GameObject _noBuses;
    [SerializeField] private GameObject _noAds;
    [SerializeField] private LevelLoader _levelLoader;
    [SerializeField] private TextMeshProUGUI _test;

    private CanvasGroup _canvasGroup;
    private bool _isBlocked;
    private RectTransform _rectTransform;
    private ParkingSpot _selectedSpot;

    [SerializeField]
    private Button _failedBtn,
        _continueBuyBtn,
        _continueByAdBtn,
        _closeContinerBtn,
        /*_openSpotBtnAd,*/
        _completedBtn,
        _doubleRewardBtnAd,
        _restartBtn,
        _settingBtn,
        _russiaBtn,
        _englishBtn,
        _turkeyBtn,
        _closesettingsBtn,
        _changeLanguageBtn,
        _noAdsBtn,
        _closeAdsBtn;

    [SerializeField] GameObject _russianFlag;
    [SerializeField] GameObject _englishFlag;
    [SerializeField] GameObject _turkeyFlag;
    [SerializeField] GameObject _germanFlag;

    [SerializeField] GameObject _disableSound;
    [SerializeField] GameObject _enableSound;

    [SerializeField]
    private Button _tryToBuyArrangeBtn, _arrangeByAdBtn, _tryToBuyJumbleBtn, _JumbleByAdBtn, _carParkingByAdBtn;

    [SerializeField] private RectTransform _arrangeRect, _jumbleRect, _carParkingRect;

    [SerializeField] private RectTransform _vipWindow, _vipChoiseWindow, _turboWindow;
    [SerializeField] public Button vipChoiseBtn;
    [SerializeField] private Button _vipBuyBtn, _vipByAdBtn;
    [SerializeField] private Button _turboByAdBtn, _turboBuyBtn;
    public event Action OnContinueBtnClicked;
    public event Action OnOpenSpotBtnAdClicked;
    public event Action OnDoubleRewardBtnAdClicked;
    public event Action OnCompleteBtnClicked;
    public event Action OnCloseContinerBtnClicked;
    private Coroutine _coroutine;

    private void Awake()
    {
        LocalizationManager.Instance.onLanguageChange += OnLanguageChange;
        _coinManager.OnCoinsChanged += OnCoinsChanged;
        if (MirraSDK.Data.HasKey("no_ads"))
        {
            _noAdsBtn.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        SetLevelText();
        _gameChecker.WinGame += OnWinGame;
        _gameChecker.LoseGame += OnLoseGame;
        _gameChecker.ContinueGame += OpenContinueWindow;

        _failedBtn.onClick.AddListener(RestartGame);
        _restartBtn.onClick.AddListener(RestartGame);
        _settingBtn.onClick.AddListener(DisableSound);
        _closesettingsBtn.onClick.AddListener(SettingsButtonClose);

        _russiaBtn.onClick.AddListener(RussiaButtonClick);
        _englishBtn.onClick.AddListener(EnglishButtonClick);
        _turkeyBtn.onClick.AddListener(TurkeyButtonClick);

        _completedBtn.onClick.AddListener(CompleteBtnClicked);
        // _openSpotBtnAd.onClick.AddListener(OpenSpotBtnAdClicked);
        _doubleRewardBtnAd.onClick.AddListener(X2GoldClicked);
        _closeContinerBtn.onClick.AddListener(CloseContinerBtnClicked);
        _changeLanguageBtn.onClick.AddListener(LanguageChangeClick);

        _tryToBuyArrangeBtn.onClick.AddListener(TryToBuyArrangeBtnClicked);
        _arrangeByAdBtn.onClick.AddListener(ArrangeByAdBtnClicked);
        _tryToBuyJumbleBtn.onClick.AddListener(TryToBuyJumbleBtnClicked);
        _JumbleByAdBtn.onClick.AddListener(JumbleByAdBtnClicked);
        _carParkingByAdBtn.onClick.AddListener(CarParkingByAdBtnClicked);

        _vipBuyBtn.onClick.AddListener(TryToBuyVipBtnClicked);
        _vipByAdBtn.onClick.AddListener(VipByAdBtnClicked);
        _turboByAdBtn.onClick.AddListener(TurboByAdBtnClicked);
        _turboBuyBtn.onClick.AddListener(TryToBuyTurboBtnClicked);
        _continueBuyBtn.onClick.AddListener(TryToBuyContinueBtnClicked);
        _continueByAdBtn.onClick.AddListener(ContinueByAdBtnClicked);

        _noAdsBtn.onClick.AddListener(NoAds);
        _closeAdsBtn.onClick.AddListener(NoAdsDisable);
        RewardedShop.OnNoAdsBuy += OnBuyNoAds;
    }


    private void OnDestroy()
    {
        _gameChecker.WinGame -= OnWinGame;
        _gameChecker.LoseGame -= OnLoseGame;
        _gameChecker.ContinueGame -= OpenContinueWindow;

        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.onLanguageChange -= OnLanguageChange;

        _failedBtn.onClick.RemoveListener(RestartGame);
        _restartBtn.onClick.RemoveListener(RestartGame);
        _settingBtn.onClick.RemoveListener(DisableSound);
        _closesettingsBtn.onClick.RemoveListener(SettingsButtonClose);

        _russiaBtn.onClick.RemoveListener(RussiaButtonClick);
        _englishBtn.onClick.RemoveListener(EnglishButtonClick);
        _turkeyBtn.onClick.RemoveListener(TurkeyButtonClick);

        _completedBtn.onClick.RemoveListener(CompleteBtnClicked);
        // _openSpotBtnAd.onClick.RemoveListener(OpenSpotBtnAdClicked);
        _doubleRewardBtnAd.onClick.RemoveListener(X2GoldClicked);
        _closeContinerBtn.onClick.RemoveListener(CloseContinerBtnClicked);
        _changeLanguageBtn.onClick.RemoveListener(LanguageChangeClick);

        _tryToBuyArrangeBtn.onClick.RemoveListener(TryToBuyArrangeBtnClicked);
        _arrangeByAdBtn.onClick.RemoveListener(ArrangeByAdBtnClicked);
        _tryToBuyJumbleBtn.onClick.RemoveListener(TryToBuyJumbleBtnClicked);
        _JumbleByAdBtn.onClick.RemoveListener(JumbleByAdBtnClicked);
        _carParkingByAdBtn.onClick.RemoveListener(CarParkingByAdBtnClicked);

        _vipBuyBtn.onClick.RemoveListener(TryToBuyVipBtnClicked);
        _vipByAdBtn.onClick.RemoveListener(VipByAdBtnClicked);
        _turboByAdBtn.onClick.RemoveListener(TurboByAdBtnClicked);
        _turboBuyBtn.onClick.RemoveListener(TryToBuyTurboBtnClicked);
        _continueBuyBtn.onClick.RemoveListener(TryToBuyContinueBtnClicked);
        _continueByAdBtn.onClick.RemoveListener(ContinueByAdBtnClicked);

        _noAdsBtn.onClick.RemoveListener(NoAds);
        _closeAdsBtn.onClick.RemoveListener(NoAdsDisable);
        RewardedShop.OnNoAdsBuy -= OnBuyNoAds;
    }

    private void NoAds()
    {
        _noAds.SetActive(true);
        AdManager.instance.OnWindowEnabled(true);
    }

    private void OnBuyNoAds()
    {
        _noAdsBtn.gameObject.SetActive(false);
        NoAdsDisable();
        AdManager.instance.OnWindowEnabled(false);
    }

    private void NoAdsDisable()
    {
        _noAds.SetActive(false);
        AdManager.instance.OnWindowEnabled(false);
    }
    private void LanguageChangeClick()
    {
        switch (MirraSDK.Language.Current)
        {
            case LanguageType.English:
                LocalizationManager.Instance.SetLanguage(LanguageType.Russian);
                _russianFlag.SetActive(true);
                _englishFlag.SetActive(false);
                _turkeyFlag.SetActive(false);
                _germanFlag.SetActive(false);
                break;
            case LanguageType.Russian:
                LocalizationManager.Instance.SetLanguage(LanguageType.Turkish);
                _russianFlag.SetActive(false);
                _englishFlag.SetActive(false);
                _turkeyFlag.SetActive(true);
                _germanFlag.SetActive(false);
                break;
            case LanguageType.Turkish:
                LocalizationManager.Instance.SetLanguage(LanguageType.German);
                _russianFlag.SetActive(false);
                _englishFlag.SetActive(false);
                _turkeyFlag.SetActive(false);
                _germanFlag.SetActive(true);
                break;
            case LanguageType.German:
                LocalizationManager.Instance.SetLanguage(LanguageType.English);
                _russianFlag.SetActive(false);
                _englishFlag.SetActive(true);
                _turkeyFlag.SetActive(false);
                _germanFlag.SetActive(false);
                break;
        }
    }

    private void DisableSound()
    {
        if (MirraSDK.Audio.Volume > 0)
        {
            MirraSDK.Audio.Volume = 0;
            _disableSound.SetActive(true);
            _enableSound.SetActive(false);
        }
        else
        {
            MirraSDK.Audio.Volume = 1;
            _disableSound.SetActive(false);
            _enableSound.SetActive(true);
        }
    }

    private void OnLanguageChange()
    {
        switch (MirraSDK.Language.Current)
        {
            case LanguageType.English:
                _russianFlag.SetActive(false);
                _englishFlag.SetActive(true);
                _turkeyFlag.SetActive(false);
                _germanFlag.SetActive(false);
                break;
            case LanguageType.Russian:
                _russianFlag.SetActive(true);
                _englishFlag.SetActive(false);
                _turkeyFlag.SetActive(false);
                _germanFlag.SetActive(false);
                break;
            case LanguageType.Turkish:
                _russianFlag.SetActive(false);
                _englishFlag.SetActive(false);
                _turkeyFlag.SetActive(true);
                _germanFlag.SetActive(false);
                break;
            case LanguageType.German:
                _russianFlag.SetActive(false);
                _englishFlag.SetActive(false);
                _turkeyFlag.SetActive(false);
                _germanFlag.SetActive(true);
                break;
        }
    }
    private void SettingsButtonOpen()
    {
        _settings.gameObject.SetActive(true);
        AdManager.instance.OnWindowEnabled(true);
    }

    private void SettingsButtonClose()
    {
        _settings.gameObject.SetActive(false);
        AdManager.instance.OnWindowEnabled(false);
    }

    private void RussiaButtonClick()
    {
        LocalizationManager.Instance.SetLanguage(LanguageType.Russian);
    }

    private void EnglishButtonClick()
    {
        LocalizationManager.Instance.SetLanguage(LanguageType.English);
    }

    private void TurkeyButtonClick()
    {
        LocalizationManager.Instance.SetLanguage(LanguageType.Turkish);
    }

    private void OnCoinsChanged(int value)
    {
        _coinText.text = value.ToString();
    }

    private void OnLoseGame()
    {
        _failed.gameObject.SetActive(true);
        AdManager.instance.OnWindowEnabled(true);
    }

    private void OnWinGame()
    {
        Analytic.LevelCompleted(MirraSDK.Data.GetInt("Level").ToString());
        _completed.gameObject.SetActive(true);
        AdManager.instance.OnWindowEnabled(true);
    }

    private void OpenContinueWindow()
    {
        _continue.gameObject.SetActive(true);
        AdManager.instance.OnWindowEnabled(true);
    }

    public void CloseContinueWindow()
    {
        _continue.gameObject.SetActive(false);
        AdManager.instance.OnWindowEnabled(false);
    }

    private void RestartGame()
    {
        Time.timeScale = 1;
       /* if (MirraSDK.Data.GetInt("Level") >= 3)
        {
            AdManager.instance.ShowInterstitial();
        }*/
        Analytic.LevelStarted(MirraSDK.Data.GetInt("Level").ToString());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        AdManager.instance.OnWindowEnabled(false);
    }

    private void ContinueBtnClicked() => OnContinueBtnClicked?.Invoke();

    private void OpenSpotBtnAdClicked()
    {
        AdManager.instance.StartRewarded(MirraSDK.Data.GetInt("Level").ToString(), "open_spot_ui", OnRewardedOpenSpot, AdNotAvailible);
    }

    private void X2GoldClicked()
    {
        AdManager.OnClose += LoadNextLevel;
        AdManager.instance.StartRewarded(MirraSDK.Data.GetInt("Level").ToString(), "x2_gold", DoubleRewardAdBtnClicked, AdNotAvailible);
    }

    private void LoadNextLevel()
    {
        AdManager.OnClose -= LoadNextLevel;
        _levelLoader.LoadNextLevel();
        AdManager.instance.OnWindowEnabled(false);
    }
    private void AdNotAvailible()
    {
        _test.text = "Ad not available "+ Time.timeScale;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(AlphaCoroutine(_adsNotAvailible));
    }

    private void VipSlotNotAvailible()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(AlphaCoroutine(_vipSlotNotEmpty));
    }

    private void NoBuses()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(AlphaCoroutine(_noBuses));
    }

    private void NoMoney()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(AlphaCoroutine(_noMoney));
    }

    private void SetLevelText()
    {
        if (MirraSDK.Data.HasKey("Level"))
        {
            _levelText.text = (MirraSDK.Data.GetInt("Level") + 1).ToString();
            _levelTextComplete.text = (MirraSDK.Data.GetInt("Level") + 1).ToString();
        }
        else
        {
            _levelText.text = "1";
            _levelTextComplete.text = "1";
        }
    }

    private IEnumerator AlphaCoroutine(GameObject obj)
    {
        obj.SetActive(true);
        _canvasGroup = obj.GetComponent<CanvasGroup>();
        _rectTransform = obj.GetComponent<RectTransform>();
        _isBlocked = true;
        float time = 1;
        while (time > 0)
        {
            _canvasGroup.alpha = time;
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x,
                Vector2.Lerp(new Vector2(-250, 100), new Vector2(-250, -150), time).y);
            time -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        obj.SetActive(false);
        _isBlocked = false;
    }

    private void OnRewardedOpenSpot() 
    {
        Analytic.BonusUsed(MirraSDK.Data.GetInt("Level").ToString(), "open_spot");
        OnOpenSpotBtnAdClicked?.Invoke();
    }
    private void CompleteBtnClicked() => OnCompleteBtnClicked?.Invoke();
    private void DoubleRewardAdBtnClicked() => OnDoubleRewardBtnAdClicked?.Invoke();
    private void CloseContinerBtnClicked() => OnCloseContinerBtnClicked?.Invoke();

    private void TryToBuyArrangeBtnClicked()
    {
        if (ParkingManager.instance.BusesInSpot.Count > 0)
        {
            if (_coinManager.TryToSpendCoins(100))
            {
                ArrangeRewarded();
            }
            else
            {
                NoMoney();
            }
        }
        else
        {
            NoBuses();
        }
    }

    private void ArrangeByAdBtnClicked()
    {
        if (ParkingManager.instance.BusesInSpot.Count > 0)
        {
            if (IsFreeUsage(_arrangeByAdBtn))
            {
                ArrangeRewarded();
            }
            else
                AdManager.instance.StartRewarded(MirraSDK.Data.GetInt("Level").ToString(), "arrange", ArrangeRewarded, AdNotAvailible);
        }
        else
        {
            NoBuses();
        }
    }

    private void ArrangeRewarded()
    {
        Analytic.BonusUsed(MirraSDK.Data.GetInt("Level").ToString(), "arrange");
        _followPath.ReorderPersonsAccordingBusInSpot();
        _arrangeRect.gameObject.SetActive(false);
    }

    private void TryToBuyJumbleBtnClicked()
    {
        if (_coinManager.TryToSpendCoins(90))
        {
            JumbleRewarded();
        }
        else
        {
            NoMoney();
        }
    }

    private void JumbleByAdBtnClicked()
    {
        if (IsFreeUsage(_JumbleByAdBtn))
        {
            JumbleRewarded();
        }
        else
            AdManager.instance.StartRewarded(MirraSDK.Data.GetInt("Level").ToString(), "jumble", JumbleRewarded, AdNotAvailible);
    }

    private void JumbleRewarded()
    {
        Analytic.BonusUsed(MirraSDK.Data.GetInt("Level").ToString(), "jumble");
        _busGenerator.ChangeBusColors();
        _jumbleRect.gameObject.SetActive(false);
    }

    private void CarParkingByAdBtnClicked()
    {
        if (IsFreeUsage(_carParkingByAdBtn))
        {
            CarParkingRevarded();
        }
        else
            AdManager.instance.StartRewarded(MirraSDK.Data.GetInt("Level").ToString(), "parkingspot", CarParkingRevarded, AdNotAvailible);
    }

    private void CarParkingRevarded()
    {
        _selectedSpot.OnRewarded();
        _carParkingRect.gameObject.SetActive(false);
        _selectedSpot = null;
    }

    public void OpenCarParkingWindow() => _carParkingRect.gameObject.SetActive(true);
    public void SelectSpot(ParkingSpot spot) => _selectedSpot = spot;

    public void ActivateVipChoise() => _vipChoiseWindow.gameObject.SetActive(true);
    public void DeactivateVipChoise() => _vipChoiseWindow.gameObject.SetActive(false);

    private void TryToBuyVipBtnClicked()
    {
        if (!_parkingManager.IsBusInVipSpot)
        {
            if (_coinManager.TryToSpendCoins(90))
            {
                VipRewarded();
            }
            else
            {
                NoMoney();
            }
        }
        else
        {
            VipSlotNotAvailible();
        }
    }

    private void VipByAdBtnClicked()
    {
        if (!_parkingManager.IsBusInVipSpot)
        {
            if (IsFreeUsage(_vipByAdBtn))
            {
                VipRewarded();
            }
            else
                AdManager.instance.StartRewarded(MirraSDK.Data.GetInt("Level").ToString(), "vip", VipRewarded, AdNotAvailible);
        }
        else
        {
            VipSlotNotAvailible();
        }
    }

    private void VipRewarded()
    {
        Analytic.BonusUsed(MirraSDK.Data.GetInt("Level").ToString(), "vip");
        _heliSystem.StartVipChoise();
        _vipWindow.gameObject.SetActive(false);
    }

    private void TryToBuyTurboBtnClicked()
    {
        if (!_turboTimer.IsTimerActive)
            if (_coinManager.TryToSpendCoins(90))
            {
                TurboRewarded();
            }
            else
            {
                NoMoney();
            }
    }

    private void TurboByAdBtnClicked()
    {
        if (!_turboTimer.IsTimerActive)
        {
            if (IsFreeUsage(_turboByAdBtn))
            {
                TurboRewarded();
            }
            else
                AdManager.instance.StartRewarded(MirraSDK.Data.GetInt("Level").ToString(), "turbo", TurboRewarded, AdNotAvailible);
        }
    }

    private void TurboRewarded()
    {
        Analytic.BonusUsed(MirraSDK.Data.GetInt("Level").ToString(), "turbo");
        _turboTimer.StartTurbo();
        _turboWindow.gameObject.SetActive(false);
    }

    private void ContinueByAdBtnClicked()
    {
        if (IsFreeUsage(_continueByAdBtn))
        {
            ContinueRewarded();
        }
        else
            AdManager.instance.StartRewarded(MirraSDK.Data.GetInt("Level").ToString(), "continue", ContinueRewarded, AdNotAvailible);
    }

    private void TryToBuyContinueBtnClicked()
    {
        if (_coinManager.TryToSpendCoins(160))
        {
            ContinueRewarded();
        }
        else
        {
            NoMoney();
        }
    }
    
    private void ContinueRewarded()
    {
        ArrangeRewarded();
        _parkingManager.OpenSpotByClickAd();
    }

    private bool IsFreeUsage(Button button)
    {
        RevardButtonChecker checker = button.GetComponent<RevardButtonChecker>();
        if (checker.CountOfUsage > 0)
        {
            checker.OnClick();
            return true;
        }
        return false;
    }
}