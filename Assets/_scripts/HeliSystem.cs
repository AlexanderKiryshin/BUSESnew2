using System.Collections.Generic;
using _scripts;
using _scripts.UI;
using UnityEngine;
using MEC;

public class HeliSystem : MonoBehaviour
{
    [SerializeField] private ParkingManager _parkingManager;
    [SerializeField] private RaycastShooter _raycastShooter;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private Helicopter _helicopter;
    [SerializeField] private Transform _vipParking;
    [SerializeField] private float _upDownBusSpeed = 4f;

    private Bus _selectedBus;
    private Vector3 _helicopterStartPosition;

    private void Start()
    {
        _helicopterStartPosition = _helicopter.transform.position;
    }
    
    public void StartVipChoise()
    {
        if(_parkingManager.IsBusInVipSpot) return;
        
        _raycastShooter.ActivateVipChoise();
        _uiManager.ActivateVipChoise();
    }

    public void SetBus(IRaycastTarget target)
    {
        if (target is Bus bus)
        {
            _selectedBus = bus;
            _uiManager.DeactivateVipChoise();
            Timing.RunCoroutine(SendBusToParking());
        }
    }

    private IEnumerator<float> SendBusToParking()
    {
        _uiManager.vipChoiseBtn.interactable = false;
        _raycastShooter.enabled = false;
        _helicopter.gameObject.SetActive(true);

        while (_helicopter.IsFlyingToTarget(_selectedBus.transform.position))
        {
            yield return Timing.WaitForOneFrame;
        }

        while (_selectedBus.transform.position.y < 2f)
        {
            _selectedBus.transform.position += Vector3.up * _upDownBusSpeed * Time.deltaTime;
            yield return Timing.WaitForOneFrame;
        }

        _selectedBus.transform.parent = _helicopter.transform;
        
        Vector3 destination = _vipParking.position;
        while (_helicopter.IsFlyingToTarget(destination))
        {
            yield return Timing.WaitForOneFrame;
        }

        _selectedBus.transform.parent = null;
        while (_selectedBus.transform.position.y > 0)
        {
            _selectedBus.transform.position -= Vector3.up * _upDownBusSpeed * Time.deltaTime;
            
            Quaternion targetRotation = Quaternion.LookRotation(_vipParking.transform.forward, Vector3.up);
            _selectedBus.transform.rotation = Quaternion.Slerp(_selectedBus.transform.rotation, targetRotation, Time.deltaTime * 20);
            yield return Timing.WaitForOneFrame;
        }
        
        _selectedBus.transform.position = destination;
        _selectedBus.transform.rotation = _vipParking.rotation;
        _selectedBus.CompleteFly();
        
        while (_helicopter.IsFlyingToTarget(_helicopterStartPosition))
        {
            yield return Timing.WaitForOneFrame;
        }

        _helicopter.gameObject.SetActive(false);
        _selectedBus = null;
        _uiManager.vipChoiseBtn.interactable = true;
        _raycastShooter.enabled = true;
    }
}