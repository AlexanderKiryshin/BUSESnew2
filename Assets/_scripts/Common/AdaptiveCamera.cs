using MirraGames.SDK;
using UnityEngine;

public class AdaptiveCamera : MonoBehaviour
{
    public float _defaultOrthographicSize = 13f; 
    public float _maxOrthographicSize = 14f;
    public Camera _cameraVFX;
    private float _defaultWidth; 

    private Camera _camera;

    void Start()
    {
        _defaultWidth = Screen.width;
        _camera = GetComponent<Camera>();

        if (_camera == null || !_camera.orthographic)
        {
            Debug.LogError("Этот скрипт должен быть прикреплен к ортографической камере!");
            enabled = false;
        }

        if (MirraSDK.Device.IsMobile)
        {
            enabled = false;
        }
    }

    void Update()
    {
        float currentWidth = Screen.width;
        float value = Mathf.Clamp01(currentWidth / _defaultWidth);
        float newOrthographicSize = Mathf.Lerp(_maxOrthographicSize, _defaultOrthographicSize, value);
        _camera.orthographicSize = newOrthographicSize;
        _cameraVFX.orthographicSize = newOrthographicSize;
    }
}