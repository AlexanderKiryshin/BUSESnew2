using MirraGames.SDK;
using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

public static class Analytic
{
    private static int _interstitialCount;
    private static int _rewardedCount;
    public static void InterstitialAvailible(string progress)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "interstitial");
        eventParameters.Add("placement", "interstitial");
        eventParameters.Add("result", "success");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        eventParameters.Add("playtime", MirraSDK.Data.GetFloat("playtime"));
        MirraSDK.Analytics.Report("video_ads_available", eventParameters);
        YG2.MetricaSend("video_ads_available", eventParameters);
    }

    public static void InterstitialStarted(string progress)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "interstitial");
        eventParameters.Add("placement", "interstitial");
        eventParameters.Add("result", "start");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        eventParameters.Add("playtime", MirraSDK.Data.GetFloat("playtime"));
        MirraSDK.Analytics.Report("video_ads_available", eventParameters);
        YG2.MetricaSend("video_ads_available", eventParameters);
    }
    public static void InterstitialWatched(string progress)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "interstitial");
        eventParameters.Add("placement", "interstitial");
        eventParameters.Add("result", "watched");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        eventParameters.Add("playtime", MirraSDK.Data.GetFloat("playtime"));
        MirraSDK.Analytics.Report("video_ads_success", eventParameters);
        YG2.MetricaSend("video_ads_success", eventParameters);
        _interstitialCount++;
    }

    public static void RewardedAvailible(string progress, string placement)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "rewarded");
        eventParameters.Add("placement", placement);
        eventParameters.Add("result", "success");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        eventParameters.Add("playtime", MirraSDK.Data.GetFloat("playtime"));
        MirraSDK.Analytics.Report("video_ads_available", eventParameters);
        YG2.MetricaSend("video_ads_available", eventParameters);
    }

    public static void RewardedStarted(string progress, string placement)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "rewarded");
        eventParameters.Add("placement", placement);
        eventParameters.Add("result", "start");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        eventParameters.Add("playtime", MirraSDK.Data.GetFloat("playtime"));
        MirraSDK.Analytics.Report("video_ads_available", eventParameters);
        YG2.MetricaSend("video_ads_available", eventParameters);
    }
    public static void RewardedWatched(string progress, string placement)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "rewarded");
        eventParameters.Add("placement", placement);
        eventParameters.Add("result", "watched");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        eventParameters.Add("playtime", MirraSDK.Data.GetFloat("playtime"));
        MirraSDK.Analytics.Report("video_ads_success", eventParameters);
        YG2.MetricaSend("video_ads_success", eventParameters);
        _rewardedCount++;
    }

    public static void BonusUsed(string progress, string bonus)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("bonusType", bonus);
        eventParameters.Add("progressMarker", progress);
        eventParameters.Add("playtime", MirraSDK.Data.GetFloat("playtime"));
        MirraSDK.Analytics.Report("bonus_used", eventParameters);
        YG2.MetricaSend("bonus_used", eventParameters);
    }
    public static void LevelStarted(int level, int version)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("level", (level+1));
        eventParameters.Add("playtime", MirraSDK.Data.GetFloat("playtime"));
        eventParameters.Add("version", version);
        MirraSDK.Analytics.Report("level_started",eventParameters);
        YG2.MetricaSend("level_started", eventParameters);
    }

    public static void LevelCompleted(int level,int version)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("level", (level + 1));     
        eventParameters.Add("playtime", MirraSDK.Data.GetFloat("playtime"));
        eventParameters.Add("interstitialCount", _interstitialCount);
        eventParameters.Add("rewardedCount", _rewardedCount);
        eventParameters.Add("version", version);
        MirraSDK.Analytics.Report("level_completed", eventParameters);
        YG2.MetricaSend("level_complete",eventParameters);
        _interstitialCount = 0;
        _rewardedCount = 0;
    }

    public static void GoldChanged(int level,float value)
    {
        ResourceChanged(level+1, "gold",value);
    }
    public static void ResourceChanged(int level,string resource,float value)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("resource", resource);
        eventParameters.Add("level", (level + 1));
        eventParameters.Add("value", value);
        eventParameters.Add("playtime", MirraSDK.Data.GetFloat("playtime"));
        MirraSDK.Analytics.Report("resource_changed", eventParameters);
        YG2.MetricaSend("resource_changed", eventParameters);
    }

    public static void BuyInApp(int level,string name)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("name", name);
        eventParameters.Add("level", (level + 1));
        MirraSDK.Analytics.Report("buy_inapp", eventParameters);
    }
}
