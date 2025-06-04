using MirraGames.SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytimeCounter : MonoBehaviour
{
    private float _sessionPlaytime;
    private float _playtime;
    public static Action<float> playtimeChanged;
    void Start()
    {
        MirraSDK.Data.SetFloat("session_playtime", 0);
        if (MirraSDK.Data.HasKey("playtime"))
        {
            _playtime = MirraSDK.Data.GetFloat("playtime");
        }
        else
        {
            MirraSDK.Data.SetFloat("playtime",0);
        }
        
        MirraSDK.Data.Save();
        StartCoroutine(TimeCounter());
    }

    private IEnumerator TimeCounter()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(10);
            _sessionPlaytime += 10;
            _playtime += 10;
            playtimeChanged?.Invoke(_playtime);
            MirraSDK.Data.SetFloat("session_playtime", _sessionPlaytime);
            MirraSDK.Data.SetFloat("playtime", _playtime);
            MirraSDK.Data.Save();
            Debug.Log("playtime " + _playtime);
        }
    }
}
