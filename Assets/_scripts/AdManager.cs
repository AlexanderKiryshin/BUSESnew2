using Assets._scripts;
using MirraGames.SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _adsText;
    [SerializeField] Image _fillAmount;
    [SerializeField] CanvasGroup _interstitialCanvas;
    [SerializeField] private float _interstitialTimer = 50;
    [SerializeField] private float _noInterstitialTimer = 180;
    [SerializeField] private float _noBannerInterval = 60;
    [SerializeField] private TextMeshProUGUI _test;
    [SerializeField] private bool _onPlayerActionAd;
    [SerializeField] private bool _showAdOnStart;
    private bool _interstitialIsReady;
    private string placement;
    private string progress;
    private Action onRewarded;
    public static AdManager instance = null;
    public static Action OnClose;
    private bool _isWindowOpen;
    private Coroutine ads;
    private float time = 0;
    private float timer = 0;
    private float _volume;
    private bool _volumeIsSet = false;
    private bool analyticIsSend;
    private float _scale;
   
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
        //ads = StartCoroutine(Ads());
        if (_showAdOnStart)
        {
            StartCoroutine(ShowAdOnStart());
        }
    }
    private IEnumerator ShowAdOnStart()
    {
        yield return new WaitUntil(() => MirraSDK.IsInitialized);
        MirraSDK.Audio.Pause = true;
        MirraSDK.Time.Scale = 0;
        MirraSDK.Ads.InvokeInterstitial(onClose: OnInterstitialClose);
    }
    public bool CanStartRewarded()
    {
        return MirraSDK.Ads.IsRewardedReady;
    }
    public void StartRewarded(string progress, string placement, Action OnRewarded, Action OnNonReady)
    {
        this.placement = placement;
        this.progress = progress;
        this.onRewarded = OnRewarded;
        _test.text = "reward 0";
        // MirraSDK.Time.Scale = 0;
        time = 0;
        MirraSDK.Audio.Pause = true;
        MirraSDK.Ads.InvokeRewarded(this.OnRewarded,onOpen, OnCloseMethod);
        Analytic.RewardedStarted(progress, placement);
    }
    private void onOpen()
    {

    }    
    private void OnRewarded()
    {
        MirraSDK.Audio.Pause = false;
        onRewarded();
        Analytic.RewardedWatched(progress, placement);
    }

    private void OnCloseMethod(bool isBool)
    {
       // _test.text = "reward 1";
        OnClose?.Invoke();
        MirraSDK.Audio.Pause = false;
        // ads = StartCoroutine(Ads());
    }
    public void ShowInterstitial()
    {
        if (MirraSDK.Ads.IsInterstitialReady)
        {
            MirraSDK.Audio.Pause = true;
            MirraSDK.Ads.InvokeInterstitial(onClose:OnInterstitialClose);
            Dictionary<string, object> placements = new Dictionary<string, object>
            {
                { "placement","interstitial" }
            };
            MirraSDK.Analytics.Report("video_ads_started", placements);
        }
    }

    public void OnPlayerAction()
    {
        if (!_onPlayerActionAd)
        {
            return;
        }
        if (time < _interstitialTimer)
        {
            return;
        }
        _interstitialIsReady = true;
        if (_isWindowOpen)
        {
            return;
        }
        if (!MirraSDK.Ads.IsInterstitialReady)
        {
            Debug.Log("video_ads_not_available");
            return;
        }
        Dictionary<string, object> placements = new Dictionary<string, object>
            {
                { "placement","interstitial" }
            };
        Analytic.InterstitialAvailible(MirraSDK.Data.GetInt("Level").ToString());
        Analytic.InterstitialStarted(MirraSDK.Data.GetInt("Level").ToString());
        MirraSDK.Audio.Pause = true;
        if (!_volumeIsSet)
        {
            _volume = MirraSDK.Audio.Volume;
            MirraSDK.Audio.Volume = 0;
            _volumeIsSet = true;
        }
        MirraSDK.Time.Scale = 0;
        timer = 0;
        time = 0;
        MirraSDK.Ads.InvokeInterstitial(onClose: OnInterstitialClose);
    }
    private void Update()
    {
        if (!MirraSDK.IsInitialized)
        {
            return;
        }
        time += Time.deltaTime;
        if (MirraSDK.Data.GetFloat("playtime") < _noInterstitialTimer)
        {
            return;
        }

        if (time < _interstitialTimer)
        {
            return;
        }
        if (_onPlayerActionAd)
        {
            return;
        }
        _interstitialIsReady = true;
        if (_isWindowOpen)
        {
            return;
        }

        _interstitialCanvas.alpha = 1;

        if (!MirraSDK.Ads.IsInterstitialReady)
        {
            Debug.Log("video_ads_not_available");
            return;
        }      
        _interstitialIsReady = false;
       
        Debug.Log("video_ads_available");
        if (!analyticIsSend)
        {
            Dictionary<string, object> placements = new Dictionary<string, object>
            {
                { "placement","interstitial" }
            };
            Analytic.InterstitialAvailible(MirraSDK.Data.GetInt("Level").ToString());
            analyticIsSend = true;
        }
        if (timer < 2)
        {
            timer += Time.deltaTime;
            _fillAmount.fillAmount = (2-timer) / 2f;
            _adsText.text = LocalizationManager.Instance.GetText("ads") + Mathf.Ceil(2-timer) + "...";
            return;
        }

        analyticIsSend = false;
        Analytic.InterstitialStarted(MirraSDK.Data.GetInt("Level").ToString());
        Debug.Log("video_ads_started");
        MirraSDK.Audio.Pause = true;
        if (!_volumeIsSet)
        {
            _volume = MirraSDK.Audio.Volume;
            MirraSDK.Audio.Volume = 0;
            _volumeIsSet = true;
        }       
        _scale= MirraSDK.Time.Scale;
        MirraSDK.Time.Scale = 0;
        timer = 0;
        time = 0;
        MirraSDK.Ads.InvokeInterstitial(onClose: OnInterstitialClose);
    }

    public void OnWindowEnabled(bool isEnabled)
    {
        _isWindowOpen = isEnabled;
    }
    private void OnInterstitialClose(bool result)
    {
        _test.text = "inter 1";
        _interstitialCanvas.alpha = 0;
        _interstitialCanvas.blocksRaycasts = false;
        MirraSDK.Audio.Pause = false;
        MirraSDK.Audio.Volume = _volume;
        _volumeIsSet = false;
        MirraSDK.Time.Scale= 1;
        Analytic.InterstitialWatched(MirraSDK.Data.GetInt("Level").ToString());
    }
}

