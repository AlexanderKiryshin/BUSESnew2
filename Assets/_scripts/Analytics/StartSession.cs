using GameAnalyticsSDK;
using MirraGames.SDK;
using UnityEngine;

public class StartSession : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameAnalytics.Initialize();
        MirraSDK.Analytics.Report("start_session");
    }
}
