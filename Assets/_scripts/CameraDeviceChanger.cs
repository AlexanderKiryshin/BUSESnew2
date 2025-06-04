using MirraGames.SDK;
using UnityEngine;

public class CameraDeviceChanger : MonoBehaviour
{
    [SerializeField] private Vector3 _cameraPositionMobile;
    [SerializeField] private Vector3 _cameraPositionPc;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _fxCamera;
    [SerializeField] private bool _testMobile = false;

    private void Awake()
    {
        if (MirraSDK.Device.IsMobile)
        {
            gameObject.transform.position = _cameraPositionMobile;
            _mainCamera.orthographicSize = 17f;
            _fxCamera.orthographicSize = 17f;
        }
        else
        {
            gameObject.transform.position = _cameraPositionPc;
            _mainCamera.orthographicSize = 13;
            _fxCamera.orthographicSize = 13;
        }

        if (_testMobile)
        {
            gameObject.transform.position = _cameraPositionMobile;
            _mainCamera.orthographicSize = 17;
            _fxCamera.orthographicSize = 17f;
        }
    }
}