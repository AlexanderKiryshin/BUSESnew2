using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using _scripts;
using _scripts.Settings;
using Random = UnityEngine.Random;
using MirraGames.SDK;
using Assets._scripts;
using System.Collections;

public class BusGenerator : MonoBehaviour
{
    [SerializeField] private BusData _busData;
    [SerializeField] private BusLoader _busLoader;
    [SerializeField] private LevelCollection _levelCollection;
    [SerializeField] private LevelLoader _levelLoader;
    [SerializeField] private ParkingManager _parkingManager;
    [SerializeField] private BusInitializer _busInitializer;
    [SerializeField] private int _startRotation;
    [SerializeField] private int[] _rotations;
    [SerializeField] private float _stepSize;
    [SerializeField] private bool _adjustRandomRotation;
    [SerializeField] private bool _generateOnStart = false;
    public Bus smallBusPrefab;
    public Vector3 SmallBusSize;
    public Bus mediumBusPrefab;
    public Vector3 MediumBusSize;
    public Bus largeBusPrefab;
    public Vector3 LargeBusSize;
    public LayerMask layerMask;

    public int smallBusCount = 0;
    public int mediumBusCount = 0;
    public int largeBusCount = 0;

    [SerializeField] private float _randomRotation = 4f;
    public Material[] Materials;
    public List<Rect> areas;
    public List<Bus> BusesInFirstArea;
    public List<Bus> BusesInSecondArea;
    public List<Bus> BusesInThirdArea;
    public List<Bus> AllBuses;


    private float _rotationY;

    private int _createdSmallBuses = 0;
    private int _createdMediumBuses = 0;
    private int _createdLargeBuses = 0;
    private BusData _currentBusData;

    private void Start()
    {
        LoadLevelSettings();
        if (_currentBusData != null)
        {
            ClearBusesList();
            _busLoader.LoadBusData(_currentBusData);
        }
        else if (_generateOnStart)
        {
            int randomIndex = Random.Range(0, _rotations.Length);
            _startRotation = _rotations[randomIndex];
            Generate();
        }

        StartCoroutine(WaitMirra());
       
    }

    public IEnumerator WaitMirra()
    {
        yield return new WaitUntil(() => MirraSDK.IsInitialized);
        _busInitializer.Initialize();
    }
    

    

    public void Generate()
    {
        ClearBusesList();
        int count = smallBusCount + mediumBusCount + largeBusCount;
        for (int i = 0; i < count;)
        {
            if (_createdSmallBuses < smallBusCount)
            {
                FillAreas(smallBusPrefab, 1, SmallBusSize);
                _createdSmallBuses++;
                i++;
            }

            if (_createdMediumBuses < mediumBusCount)
            {
                FillAreas(mediumBusPrefab, 1, MediumBusSize);
                _createdMediumBuses++;
                i++;
            }

            if (_createdLargeBuses < largeBusCount)
            {
                FillAreas(largeBusPrefab, 1, LargeBusSize);
                _createdLargeBuses++;
                i++;
            }
        }
    }

    public void ClearBusesList()
    {
        foreach (var bus in BusesInFirstArea)
        {
            if (bus != null)
                DestroyImmediate(bus.gameObject);
        }

        foreach (var bus in BusesInSecondArea)
        {
            if (bus != null)
                DestroyImmediate(bus.gameObject);
        }

        foreach (var bus in BusesInThirdArea)
        {
            if (bus != null)
                DestroyImmediate(bus.gameObject);
        }

        foreach (var bus in AllBuses)
        {
            if (bus != null)
                DestroyImmediate(bus.gameObject);
        }

        _createdSmallBuses = 0;
        _createdMediumBuses = 0;
        _createdLargeBuses = 0;
        BusesInFirstArea.Clear();
        BusesInSecondArea.Clear();
        BusesInThirdArea.Clear();
        AllBuses.Clear();
    }

    public void FillAreas(Bus busPrefab, int count, Vector3 busSize)
    {
        for (int j = 0; j < areas.Count; j++)
        {
            int busesPlaced = 0;

            for (int i = 0; i < count; i++)
            {
                _rotationY = Random.Range(0, 2) == 0 ? _startRotation : -_startRotation;
                _rotationY += Random.Range(-_randomRotation, _randomRotation);

                Vector2 position = GetRandomPositionInArea(areas[j], busSize, _rotationY, _stepSize);
                if (position == Vector2.zero)
                    break;

                Bus bus = Instantiate(busPrefab, new Vector3(position.x, 0, position.y),
                    Quaternion.Euler(0, _rotationY, 0));
                AllBuses.Add(bus);
                bus.meshRendererBody.materials[0] = Materials[j];
                CheckAndAdjustRotation(bus.transform);
                switch (j)
                {
                    case 0:
                        BusesInFirstArea.Add(bus);
                        break;
                    case 1:
                        BusesInSecondArea.Add(bus);
                        break;
                    case 2:
                        BusesInThirdArea.Add(bus);
                        break;
                }

                busesPlaced++;
            }


            count -= busesPlaced;
            if (count <= 0)
            {
                break;
            }

            ;
        }
    }

    Vector2 GetRandomPositionInArea(Rect area, Vector3 busSize, float busRotationY)
    {
        int attempts = 1000;

        for (int i = 0; i < attempts; i++)
        {
            float x = Random.Range(area.xMin, area.xMax);
            float y = Random.Range(area.yMin, area.yMax);

            Vector3 position = new Vector3(x, 0, y);
            Quaternion rotation = Quaternion.Euler(0, busRotationY, 0);

            if (!IsOverlapping(position, busSize, rotation))
            {
                _rotationY = busRotationY;
                return new Vector2(x, y);
            }

            rotation = Quaternion.Euler(0, -busRotationY, 0);
            if (!IsOverlapping(position, busSize, rotation))
            {
                _rotationY = -busRotationY;
                return new Vector2(x, y);
            }
        }

        return Vector2.zero;
    }

    //По сетке
    public Vector2 GetRandomPositionInArea(Rect area, Vector3 busSize, float busRotationY, float stepSize)
    {
        for (float x = area.xMin; x <= area.xMax; x += stepSize)
        {
            for (float y = area.yMin; y <= area.yMax; y += stepSize)
            {
                Vector3 position = new Vector3(x, 0, y);
                Quaternion rotation = Quaternion.Euler(0, busRotationY, 0);

                if (!IsOverlapping(position, busSize, rotation))
                {
                    _rotationY = busRotationY;
                    return new Vector2(x, y);
                }

                rotation = Quaternion.Euler(0, -busRotationY, 0);
                if (!IsOverlapping(position, busSize, rotation))
                {
                    _rotationY = -busRotationY;
                    return new Vector2(x, y);
                }
            }
        }

        return Vector2.zero;
    }


    private void CheckAndAdjustRotation(Transform objTransform)
    {
        Vector3 position = objTransform.position;
        Vector3 rotation = objTransform.eulerAngles;

        if (position.z < 0)
        {
            if (_adjustRandomRotation)
            {
                rotation.y += 180; //Random.Range(0, _randomRotaionParameter) == 0 ? 180 : 0; // рандомные моменты
            }

            objTransform.eulerAngles = rotation;
        }

        // if (position.z > 0)
        // {
        //     if (_adjustRandomRotation)
        //     {
        //         rotation.y += Random.Range(0, _randomRotaionParameter) == 0 ? 0 : 180; // рандомные моменты
        //     }
        //
        //     objTransform.eulerAngles = rotation;
        // }
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle > 180) angle -= 360;
        return angle;
    }

    private bool IsOverlapping(Vector3 position, Vector3 size, Quaternion rotation)
    {
        Collider[] colliders = Physics.OverlapBox(position, size / 2, rotation, layerMask);

        return colliders.Length > 0;
    }

    private bool IsSequencesFinded(List<Bus> buses)
    {
        List<Bus> sequence = new List<Bus>();

        while (AllBuses.Count > 0)
        {
            foreach (var bus in AllBuses)
            {
                bus.CheckWay();
            }

            Bus selectedBus = AllBuses.Find(bus => bus.IsWayClear);

            if (selectedBus != null)
            {
                selectedBus.SetCollider(false);
                sequence.Add(selectedBus);
                AllBuses.Remove(selectedBus);
            }
            else
            {
                break;
            }
        }

        foreach (var bus in sequence)
        {
            bus.SetCollider(true);
        }

        foreach (var bus in AllBuses)
        {
            bus.SetCollider(true);
        }

        if (AllBuses.Count != 0)
        {
            Debug.Log(AllBuses.Count);
            return false;
        }

        AllBuses = sequence;
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Rect area in areas)
        {
            DrawRectGizmo(area, Color.red);
        }
    }

    private void DrawRectGizmo(Rect rect, Color color)
    {
        Gizmos.color = color;
        Vector3 position = new Vector3(rect.x + rect.width / 2, 0, rect.y + rect.height / 2);
        Vector3 size = new Vector3(rect.width, 0.1f, rect.height);
        Gizmos.DrawWireCube(position, size);
    }

    private void LoadLevelSettings()
    {
        int level = _levelLoader.Level;
        bool hasMatch = false;
        for (int i = 0; i < _levelCollection.levels.Count; i++)
        {
            if (level == i)
            {
                hasMatch = true;
                _currentBusData = _levelCollection.levels[i].busData;
                if (_currentBusData != null)
                    return;
                smallBusCount = _levelCollection.levels[i].SmallBusCount;
                mediumBusCount = _levelCollection.levels[i].MediumBusCount;
                largeBusCount = _levelCollection.levels[i].LargeBusCount;
                return;
            }
        }

        if (!hasMatch)
        {
            smallBusCount = _levelCollection.levels[_levelCollection.levels.Count - 1].SmallBusCount;
            mediumBusCount = _levelCollection.levels[_levelCollection.levels.Count - 1].MediumBusCount;
            largeBusCount = _levelCollection.levels[_levelCollection.levels.Count - 1].LargeBusCount;
        }
    }

    [ContextMenu("ChangeBusColors")]
    public void ChangeBusColors()
    {
        List<Bus> allBuses = new List<Bus>();
        allBuses.AddRange(BusesInFirstArea);
        allBuses.AddRange(BusesInSecondArea);
        allBuses.AddRange(BusesInThirdArea);


        var busesByType = allBuses.GroupBy(bus => bus.Type).ToList();

        HashSet<Bus> usedBuses = new HashSet<Bus>();

        foreach (var group in busesByType)
        {
            var buses = group.ToList();

            buses = buses.Where(bus => !usedBuses.Contains(bus)).ToList();

            if (buses.Count < 2) continue;

            var busPairs = GetBusPairs(buses);

            foreach (var pair in busPairs)
            {
                SwapBusColors(pair.Item1, pair.Item2);

                usedBuses.Add(pair.Item1);
                usedBuses.Add(pair.Item2);
            }
        }
    }

    private List<(Bus, Bus)> GetBusPairs(List<Bus> buses)
    {
        List<(Bus, Bus)> pairs = new List<(Bus, Bus)>();

        for (int i = 0; i < buses.Count; i++)
        {
            for (int j = i + 1; j < buses.Count; j++)
            {
                pairs.Add((buses[i], buses[j]));
            }
        }

        return pairs;
    }

    private void SwapBusColors(Bus bus1, Bus bus2)
    {
        Material tempColor = bus1.Color;
        ColorType colorType = bus1.ColorType;
        bus1.Color = bus2.Color;
        bus1.ColorType = bus2.ColorType;
        bus2.Color = tempColor;
        bus2.ColorType = colorType;
    }

    public void RemoveBusFromZones(Bus bus)
    {
        if (BusesInFirstArea.Contains(bus))
            BusesInFirstArea.Remove(bus);
        else if (BusesInSecondArea.Contains(bus))
            BusesInSecondArea.Remove(bus);
        else if (BusesInSecondArea.Contains(bus))
            BusesInFirstArea.Remove(bus);
    }

    public void LoadBusData()
    {
        if(_busData != null)
            _busLoader.LoadBusData(_busData);
    }

    public void SaveBusData()
    {
        if (_busData != null)
            _busLoader.SaveBusData(_busData);
    }

    public void GenerateSolvablePuzzle()
    {
        ClearBusesList();
        List<(Bus prefab, int count, Vector3 size)> busTypes = new List<(Bus, int, Vector3)>
        {
            (smallBusPrefab, smallBusCount, SmallBusSize),
            (mediumBusPrefab, mediumBusCount, MediumBusSize),
            (largeBusPrefab, largeBusCount, LargeBusSize)
        };

        // Сначала внутренняя зона, потом внешние
        for (int zoneIndex = 0; zoneIndex < areas.Count; zoneIndex++)
        {
            foreach (var (busPrefab, count, busSize) in busTypes)
            {
                int placed = 0;
                int maxAttempts = 10;
                int attempts = 0;
                Bus bus = null;
                while (placed < count && attempts < maxAttempts)
                {
                    float rotation = GetRandomRotationForZone(zoneIndex);
                    Vector2 pos2D = GetRandomPositionInArea(areas[zoneIndex], busSize, rotation, _stepSize);
                    if (pos2D == Vector2.zero)
                    {
                        attempts++;
                        continue;
                    }

                    Vector3 pos = new Vector3(pos2D.x, 0, pos2D.y);
                    if (bus == null)
                    {
                        bus = Instantiate(busPrefab, pos, Quaternion.Euler(0, rotation, 0));
                    }
                    AllBuses.Add(bus);
                    bus.meshRendererBody.materials[0] = Materials[zoneIndex];
                    AddBusToZoneList(bus, zoneIndex);

                    if (IsPuzzleSolvable())
                    {
                        placed++;
                    }
                    else
                    {
                        // Удаляем автобус, если он мешает решению
                        RemoveBusFromZones(bus);
                        AllBuses.Remove(bus);
                        DestroyImmediate(bus.gameObject);
                    }
                    attempts++;
                }
                if (placed < count)
                {
                    Debug.LogError($"Не удалось разместить {count} автобусов типа {busPrefab.name} в зоне {zoneIndex + 1}");
                }
            }
        }
    }

    private float GetRandomRotationForZone(int zoneIndex)
    {
        // Здесь можно сделать более гибко, если нужно
        if (_rotations != null && _rotations.Length > 0)
            return _rotations[Random.Range(0, _rotations.Length)];
        return _startRotation;
    }

    private void AddBusToZoneList(Bus bus, int zoneIndex)
    {
        switch (zoneIndex)
        {
            case 0: BusesInFirstArea.Add(bus); break;
            case 1: BusesInSecondArea.Add(bus); break;
            case 2: BusesInThirdArea.Add(bus); break;
        }
    }

    // Проверка решаемости: можно ли убрать все автобусы по одному, если остальные стоят на месте
    private bool IsPuzzleSolvable()
    {
        // Копируем список автобусов
        List<Bus> busesCopy = new List<Bus>(AllBuses);
        HashSet<Bus> removed = new HashSet<Bus>();

        while (busesCopy.Count > 0)
        {
            bool found = false;
            foreach (var bus in busesCopy)
            {
                if (CanBusExit(bus, busesCopy))
                {
                    removed.Add(bus);
                    busesCopy.Remove(bus);
                    found = true;
                    break;
                }
            }
            if (!found)
                return false; // Остались заблокированные автобусы
        }
        return true;
    }

    // Проверка: может ли автобус уехать из зоны (нет ли на пути других автобусов)
    private bool CanBusExit(Bus bus, List<Bus> allBuses)
    {
        // Здесь предполагается, что автобус едет по своей оси вперёд до выхода за пределы всех зон
        // Можно реализовать через Raycast или проверку коллизий с другими автобусами
        Vector3 dir = bus.transform.forward;
        Vector3 pos = bus.transform.position;
        float maxDistance = 100f; // Достаточно, чтобы выехать за пределы

        Ray ray = new Ray(pos, dir);
        foreach (var other in allBuses)
        {
            if (other == bus) continue;
            // Проверяем, пересекает ли луч коллайдер другого автобуса
            Collider col = other.GetComponent<Collider>();
            if (col != null && col.bounds.IntersectRay(ray))
            {
                // На пути есть другой автобус
                return false;
            }
        }
        // Проверяем, выйдет ли автобус за пределы всех зон
        Vector3 exitPoint = pos + dir * maxDistance;
        bool outside = true;
        foreach (var area in areas)
        {
            if (area.Contains(new Vector2(exitPoint.x, exitPoint.z)))
            {
                outside = false;
                break;
            }
        }
        return outside;
    }
}