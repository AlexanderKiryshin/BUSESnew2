using MirraGames.SDK;
using System.Collections.Generic;
using UnityEngine;

public static class Analytic
{
    public static void InterstitialAvailible(string progress)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "interstitial");
        eventParameters.Add("placement", "interstitial");
        eventParameters.Add("result", "success");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        MirraSDK.Analytics.Report("video_ads_available", eventParameters);

    }

    public static void InterstitialStarted(string progress)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "interstitial");
        eventParameters.Add("placement", "interstitial");
        eventParameters.Add("result", "start");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        MirraSDK.Analytics.Report("video_ads_available", eventParameters);
    }
    public static void InterstitialWatched(string progress)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "interstitial");
        eventParameters.Add("placement", "interstitial");
        eventParameters.Add("result", "watched");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        MirraSDK.Analytics.Report("video_ads_success", eventParameters);
    }

    public static void RewardedAvailible(string progress, string placement)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "rewarded");
        eventParameters.Add("placement", placement);
        eventParameters.Add("result", "success");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        MirraSDK.Analytics.Report("video_ads_available", eventParameters);

    }

    public static void RewardedStarted(string progress, string placement)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "rewarded");
        eventParameters.Add("placement", placement);
        eventParameters.Add("result", "start");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        MirraSDK.Analytics.Report("video_ads_available", eventParameters);
    }
    public static void RewardedWatched(string progress, string placement)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("adType", "rewarded");
        eventParameters.Add("placement", placement);
        eventParameters.Add("result", "watched");
        eventParameters.Add("internetConnection", Application.internetReachability != NetworkReachability.NotReachable);
        eventParameters.Add("progressMarker", progress);
        MirraSDK.Analytics.Report("video_ads_success", eventParameters);
    }

    public static void BonusUsed(string progress, string bonus)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("bonusType", bonus);
        eventParameters.Add("progressMarker", progress);
        MirraSDK.Analytics.Report("bonus_used", eventParameters);
    }
    public static void LevelStarted(string level)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("level", level);
        MirraSDK.Analytics.Report("level_started",eventParameters);
    }

    public static void LevelCompleted(string level)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("level", level);
        MirraSDK.Analytics.Report("level_comleted", eventParameters);
    }

    public static void GoldChanged(string level,float value)
    {
        ResourceChanged(level, "gold",value);
    }
    public static void ResourceChanged(string level,string resource,float value)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("resource", resource);
        eventParameters.Add("level", level);
        eventParameters.Add("value", value);
        MirraSDK.Analytics.Report("resource_changed", eventParameters);
    }

    public static void BuyInApp(string level,string name)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>();
        eventParameters.Add("name", name);
        eventParameters.Add("level", level);
        MirraSDK.Analytics.Report("buy_inapp", eventParameters);
    }
}
