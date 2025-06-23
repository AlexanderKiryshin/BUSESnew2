using MirraGames.SDK;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameIsReady : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitForGameReady());                
    }

    private IEnumerator WaitForGameReady()
    {
        yield return new WaitUntil(() => MirraSDK.IsInitialized);
        MirraSDK.Analytics.GameIsReady();
        var operation = SceneManager.LoadSceneAsync("bus");
        operation.completed += (AsyncOperation obj) =>
        {
            MirraSDK.Analytics.GameplayStart();
        };
    }
}
