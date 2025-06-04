using MirraGames.SDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameIsReady : MonoBehaviour
{
    void Start()
    {
        MirraSDK.Analytics.GameIsReady();
        
        var operation=SceneManager.LoadSceneAsync("bus");
        operation.completed += (AsyncOperation obj) =>
        {
            MirraSDK.Analytics.GameplayStart();
        };
    }
}
