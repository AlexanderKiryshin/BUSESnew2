using Assets._scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ParkingManager : MonoBehaviour
{
    [SerializeField] private PersonLoader _personLoader;
    [SerializeField] private UiManager _uiManager;
    public static ParkingManager instance = null;
    public Transform AwayPoint;
    public ParkingSpot[] parkingSpots;
    public List<ParkingSpot> availableParkingSpots = new List<ParkingSpot>();
    public List<Bus> BusesInSpot;
    public bool IsBusInVipSpot;
    private bool[] _isSpotesOccupied;

    public event Action<ColorType, bool> BusInSpotChanged;


    private void Start()
    {
        // _uiManager.OnOpenSpotBtnAdClicked += OpenSpotByClickAd;
        // _uiManager.OnContinueBtnClicked += TryToBuySpot;

        _isSpotesOccupied = new bool[parkingSpots.Length];

        for (int i = 0; i < parkingSpots.Length; i++)
        {
            if (!parkingSpots[i]._isCanOppenedByAd)
                availableParkingSpots.Add(parkingSpots[i]);
        }
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        // _uiManager.OnOpenSpotBtnAdClicked -= OpenSpotByClickAd;
        // _uiManager.OnContinueBtnClicked -= TryToBuySpot;
    }

    public ParkingSpot GetAvailableSpot()
    {
        for (int i = 0; i < availableParkingSpots.Count; i++)
        {
            if (!_isSpotesOccupied[i])
            {
                return availableParkingSpots[i];
            }
        }

        return null;
    }

    public bool IsFreeSpotRemains()
    {
        for (int i = 0; i < availableParkingSpots.Count; i++)
        {
            if (_isSpotesOccupied[i] == false)
            {
                return true;
            }
        }

        return false;
    }

    public void MarkSpotOccupied(ParkingSpot spot, Bus bus)
    {
        for (int i = 0; i < availableParkingSpots.Count; i++)
        {
            if (availableParkingSpots[i] == spot)
            {
                _isSpotesOccupied[i] = true;
                break;
            }
        }
    }

    public void BusArrived(Bus bus)
    {
        BusesInSpot.Add(bus);
        BusInSpotChanged?.Invoke(bus.ColorType, true);
    }

    public void BusFlightArrived(Bus bus)
    {
        IsBusInVipSpot = true;
        BusesInSpot.Add(bus);
        BusInSpotChanged?.Invoke(bus.ColorType, true);
    }

    public void FreeSpot(Bus bus)
    {
        ParkingSpot spot = availableParkingSpots[1]; ////
        for (int i = 0; i < availableParkingSpots.Count; i++)
        {
            if (Vector3.Distance(availableParkingSpots[i].transform.position, bus.transform.position) <= 0.2f)
            {
                spot = availableParkingSpots[i];
                break;
            }
        }

        for (int i = 0; i < availableParkingSpots.Count; i++)
        {
            if (availableParkingSpots[i] == spot)
            {
                List<Vector3> Path = new List<Vector3>();
                Vector3 firstPoint = availableParkingSpots[i].transform.position + new Vector3(1, 0, -4);
                Path.Add(firstPoint);
                Path.Add(AwayPoint.position);
                bus.LeaveParking(Path);
                BusesInSpot.Remove(bus);
                _isSpotesOccupied[i] = false;
                
                BusInSpotChanged?.Invoke(bus.ColorType, false);
                SoundManager.instance.PlayBusRuningSound();
                break;
            }
        }
    }

    public void FreeVipSpot(Bus bus)
    {
        List<Vector3> Path = new List<Vector3>();
        Vector3 firstPoint = bus.transform.position + new Vector3(1, 0, -4);
        Path.Add(firstPoint);
        Path.Add(AwayPoint.position);
        bus.LeaveParking(Path);
        BusesInSpot.Remove(bus);
        IsBusInVipSpot = false;
        BusInSpotChanged?.Invoke(bus.ColorType, false);
        SoundManager.instance.PlayBusRuningSound();
    }

    public void OpenSpotByClickSpot(ParkingSpot spot)
    {
        for (int i = 0; i < parkingSpots.Length; i++)
        {
            if (parkingSpots[i] == spot)
            {
                availableParkingSpots.Add(parkingSpots[i]);
                return;
            }
        }
    }

    public void OpenSpotByClickAd()
    {
        for (int i = 0; i < parkingSpots.Length; i++)
        {
            if (parkingSpots[i]._isCanOppenedByAd)
            {
                parkingSpots[i]._isCanOppenedByAd = false;
                parkingSpots[i].CheckSpot();
                availableParkingSpots.Add(parkingSpots[i]);
                _uiManager.CloseContinueWindow();
                return;
            }
        }
    }

    private void TryToBuySpot()
    {
        if (CoinManager.instance.TryToSpendCoins(160))
        {
            OpenSpotByClickAd();
        }

    }
}