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
        MirraSDK.Ads.InvokeRewarded(this.OnRewarded,onOpen, OnCloseMethod);
        Analytic.RewardedStarted(progress, placement);
    }
    private void onOpen()
    {

    }    
    private void OnRewarded()
    {
        onRewarded();
        Analytic.RewardedWatched(progress, placement);
    }

    private void OnCloseMethod(bool isBool)
    {
       // _test.text = "reward 1";
        MirraSDK.Time.Scale = 1;
        OnClose?.Invoke();
       // ads = StartCoroutine(Ads());
    }
    public void ShowInterstitial()
    {
        if (MirraSDK.Ads.IsInterstitialReady)
        {
            MirraSDK.Ads.InvokeInterstitial(OnInterstitialClose);
            Dictionary<string, object> placements = new Dictionary<string, object>
            {
                { "placement","interstitial" }
            };
            MirraSDK.Analytics.Report("video_ads_started", placements);
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (MirraSDK.Data.GetFloat("playtime") < _noInterstitialTimer)
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

        _interstitialCanvas.alpha = 1;

        if (!MirraSDK.Ads.IsInterstitialReady)
        {
            Debug.Log("video_ads_not_available");
            return;
        }
        _interstitialIsReady = false;
        Debug.Log("video_ads_available");
        if (timer < 2)
        {
            timer += Time.deltaTime;
            _fillAmount.fillAmount = (2-timer) / 2f;
            _adsText.text = LocalizationManager.Instance.GetText("ads") + Mathf.Ceil(2-timer) + "...";
            return;
        }
        timer = 0;
        time = 0;

        Debug.Log("video_ads_started");
        OnInterstitialClose();
        MirraSDK.Ads.InvokeInterstitial(OnInterstitialClose);

    }

    public void OnWindowEnabled(bool isEnabled)
    {
        _isWindowOpen = isEnabled;
    }
    private void OnInterstitialClose()
    {
        _test.text = "inter 1";
        _interstitialCanvas.alpha = 0;
        _interstitialCanvas.blocksRaycasts = false;
        Debug.Log("closed");
        MirraSDK.Time.Scale = 1;
        MirraSDK.Audio.Pause = false;
    }
}

