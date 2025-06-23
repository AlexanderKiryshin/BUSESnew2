using Assets._scripts;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameChecker : MonoBehaviour
{
    [SerializeField] private BusRemover _busRemover;
    [SerializeField] private FollowPath _followPath;
    [SerializeField] private PersonLoader _personLoader;
    [SerializeField] private ParkingManager _parkingManager;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private int maxBuses = 7;

    private int _busLeftCount;
    private int _personLeftCount;
    private Person _firstPerson;
    private List<ColorType> _busInSpotColors = new List<ColorType>();

    public event Action WinGame;
    public event Action LoseGame;
    public event Action ContinueGame;

    private void Start()
    {
        _busRemover.OnBusDestroy += OnOnBusDestroy;
        _followPath.PersonsLeftChanged += OnPersonsLeftChanged;
        _personLoader.FirstPersonChancged += OnFirstPersonChancged;
        _parkingManager.BusInSpotChanged += OnBusInSpotChanged;
        _uiManager.OnCloseContinerBtnClicked += Lose;
    }

    private void OnDisable()
    {
        _busRemover.OnBusDestroy -= OnOnBusDestroy;
        _followPath.PersonsLeftChanged -= OnPersonsLeftChanged;
        _personLoader.FirstPersonChancged -= OnFirstPersonChancged;
        _parkingManager.BusInSpotChanged -= OnBusInSpotChanged;
        _uiManager.OnCloseContinerBtnClicked -= Lose;
    }

    public void SetBusCount(int busCount) => _busLeftCount = busCount;

    private void OnFirstPersonChancged(Person person)
    {
        _firstPerson = person;
        Invoke(nameof(CheckLoseState), 0.25f);
    }

    private void OnBusInSpotChanged(ColorType color, bool isArrived)
    {
        if (!isArrived)
        {
            _busInSpotColors.Remove(color);
        }
        else
        {
            _busInSpotColors.Add(color);
            CheckLoseState();
        }
    }

    private void OnPersonsLeftChanged(int value)
    {
        _personLeftCount = value;
        CheckWinState(_personLeftCount, _busLeftCount);
    }


    private void OnOnBusDestroy()
    {
        _busLeftCount--;
        CheckWinState(_personLeftCount, _busLeftCount);
    }

    private void CheckWinState(int personsLeft, int busLeftCount)
    {
        if (personsLeft == 0 && busLeftCount == 0)
        {
            WinGame?.Invoke();
        }
    }

    private void CheckLoseState()
    {
        if (_firstPerson == null || _busInSpotColors.Count == 0 || _personLoader.CoroutineFreeSpot != null)
            return;

        if (!_parkingManager.IsFreeSpotRemains())
        {
            int sum = _busInSpotColors.Count;
            if (_parkingManager.IsBusInVipSpot)
                sum -= 1;
            if (sum != _parkingManager.availableParkingSpots.Count)
                return;

            bool hasMatch = false;
            foreach (var color in _busInSpotColors)
            {
                if (color == _firstPerson.ColorType)
                {
                    hasMatch = true;
                    break;
                }
            }

            if (!hasMatch)
            {
                if (_parkingManager.IsBusInVipSpot)
                    maxBuses += 1;
                if (_busInSpotColors.Count != maxBuses)
                {
                    ContinueGame?.Invoke();
                    return;
                }

                LoseGame?.Invoke();
            }
        }
    }

    private void Lose() => LoseGame?.Invoke();
}