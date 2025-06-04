using UnityEngine;

public class BusInitializer : MonoBehaviour
{
    [SerializeField] private ParkingManager _parkingManager;
    [SerializeField] private ColorManager _colorManager;
    [SerializeField] private Color[] _colors;
    [SerializeField] private BusGenerator _busGenerator;
    [SerializeField] private GameChecker _gameChecker;

    private Color[] _colorsInFirstArea;
    private Color[] _colorsInSecondArea;
    private Color[] _colorsInThirdArea;

    public void Initialize()
    {
        foreach (var bus in _busGenerator.BusesInFirstArea)
        {
            bus.Initialize(_parkingManager, _busGenerator);
            bus.SetColliderXSize(0.85f);
        }
        foreach (var bus in _busGenerator.BusesInSecondArea)
        {
            bus.Initialize(_parkingManager, _busGenerator);
            bus.SetColliderXSize(0.85f);
        }
        foreach (var bus in _busGenerator.BusesInThirdArea)
        {
            bus.Initialize(_parkingManager, _busGenerator);
            bus.SetColliderXSize(0.85f);
        }
        //int count = _busGenerator.smallBusCount + _busGenerator.mediumBusCount + _busGenerator.largeBusCount;
        _gameChecker.SetBusCount(_busGenerator.AllBuses.Count);
        _colorManager.AssignBusColors(_busGenerator);
    }
}