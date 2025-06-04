using System.Collections.Generic;
using UnityEngine;
using MEC;

public class DoorOpenCotrutine : MonoBehaviour
{
    [SerializeField] private Transform _door;
    [SerializeField] private float _openAngle = 90f;
    [SerializeField] private float _openTime = 1f;

    private Quaternion _startRotation;
    private Quaternion _endRotation;
    private bool _isOpening = false;
    
    void Start()
    {
            _startRotation = _door.localRotation;
            _endRotation = Quaternion.Euler(0, 0, _openAngle) * _startRotation;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!_isOpening)
                Timing.RunCoroutine(OpenDoor());
        }
    }

    IEnumerator<float> OpenDoor()
    {
        _isOpening = true;
        float elapsedTime = 0f;

        while (elapsedTime < _openTime)
        {
            _door.localRotation = Quaternion.Lerp(_startRotation, _endRotation, elapsedTime / _openTime);
            elapsedTime += Time.deltaTime;
            yield return Timing.WaitForOneFrame;
        }

        _door.localRotation = _endRotation;
    }
}