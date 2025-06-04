using System.Collections.Generic;
using System.Linq;
using _scripts.Settings;
using UnityEditor;
using UnityEngine;
using MirraGames.SDK;
using MirraGames.SDK.Common;

public enum Difficulty
{
    Easy = 0,
    Hard = 1
}

[SelectionBase]
public class ColorManager : MonoBehaviour
{
    [SerializeField] private Material _hidedBusMaterial;
    [SerializeField] private FollowPath _followPath;
    [SerializeField] private LevelCollection _levelCollection;
    [SerializeField] private LevelLoader _levelLoader;
    [Range(1, 8)] public int NumberOfColors = 8;
    [Range(5, 100)] public int ZoneMixPercentage = 10;

    public List<Material> GeneratedColors;
    public List<Bus> BusSequence;
    public List<Material> ColorSequencePerson;

    private List<Material> _firstZoneColors;
    private List<Material> _secondZoneColors;
    private List<Material> _thirdZoneColors;
    private BusGenerator _busGenerator;
    private int _randomDelimeter;

    private Difficulty _difficulty;

    public void AssignBusColors(BusGenerator busGenerator)
    {
        LoadDifficultyData();
        _randomDelimeter = Random.Range(2, 4);
        GeneratedColors = GeneratedColors.OrderBy(x => Random.value).ToList();
        GeneratedColors = GeneratedColors.Take(NumberOfColors).ToList();

        _busGenerator = busGenerator;
        // _firstZoneColors = GetZoneColors(GeneratedColors, new List<Material>(), ZoneMixPercentage);
        // _secondZoneColors = GetZoneColors(GeneratedColors, _firstZoneColors, ZoneMixPercentage);
        // _thirdZoneColors = GetZoneColors(GeneratedColors, _secondZoneColors, ZoneMixPercentage);

        AssignBusColors(busGenerator.BusesInFirstArea, GeneratedColors);
        AssignBusColors(busGenerator.BusesInSecondArea, GeneratedColors);
        AssignBusColors(busGenerator.BusesInThirdArea, GeneratedColors);
        BusSequence = FindSequences(busGenerator.AllBuses);
        switch (_difficulty)
        {
            case Difficulty.Easy:
                ColorSequencePerson = GenerateColorSequenceForPeopleEase(BusSequence, _randomDelimeter);
                break;
            case Difficulty.Hard:
                ColorSequencePerson = GenerateColorSequenceForPeopleHard(BusSequence, _randomDelimeter);
                break;
        }

        _followPath.Initialize();
        AssignHidedBuses();
    }

    private void AssignHidedBuses()
    {
        if (_levelLoader.Level >= 8 )
        {
            int randomCount = Random.Range(3, 7);
            for (int i = 0; i < randomCount; i++)
            {
                int index = Random.Range(0, 2);
                if (index == 0)
                {
                    if (_busGenerator.BusesInFirstArea.Count > 0)
                    {
                        int randomindex = Random.Range(0, _busGenerator.BusesInFirstArea.Count-1);
                        _busGenerator.BusesInFirstArea[randomindex].HideBus(_hidedBusMaterial);
                    }
                }
                else
                {
                    if (_busGenerator.BusesInSecondArea.Count > 0)
                    {
                        int randomindex = Random.Range(0, _busGenerator.BusesInSecondArea.Count-1);
                        _busGenerator.BusesInSecondArea[randomindex].HideBus(_hidedBusMaterial);
                    }
                }
            }
        }
    }

    private List<Material> GetZoneColors(List<Material> allColors, List<Material> excludedColors, int mixPercentage)
    {
        var availableColors = allColors.Except(excludedColors).ToList();
        int mixCount = Mathf.CeilToInt(allColors.Count * mixPercentage / 100f);

        var zoneColors = new List<Material>();
        zoneColors.AddRange(availableColors.Take(allColors.Count - mixCount));
        zoneColors.AddRange(excludedColors.Take(mixCount));

        zoneColors = zoneColors.Where(color => allColors.Contains(color)).Distinct().ToList();

        return zoneColors;
    }

    private void AssignBusColors(List<Bus> buses, List<Material> zoneColors)
    {
        foreach (var bus in buses)
        {
            // Выбираем случайный цвет из доступных цветов зоны
            int colorIndexInZone = Random.Range(0, zoneColors.Count);
            Material selectedColor = zoneColors[colorIndexInZone];

            bus.Color = selectedColor;
            // bus.meshRendererBody.materials[0].color = selectedColor;
            // bus.meshRendererTop.materials[0].color = selectedColor;
            bus.meshRendererBody.material = selectedColor;
            bus.meshRendererTop.material = selectedColor;
            // switch (bus.Type)
            // {
            //     case BusType.Small:
            //         break;
            //     case BusType.Medium:
            //         bus.meshRendererBody.materials[0].color = selectedColor;
            //         bus.meshRendererTop.materials[0].color = selectedColor;
            //         break;
            //     case BusType.Large:
            //         bus.meshRendererBody.materials[1].color = selectedColor;
            //         bus.meshRendererTop.materials[1].color = selectedColor;
            //         break;
            // }
        }
    }

    private List<Bus> FindSequences(List<Bus> buses)
    {
        List<Bus> sequence = new List<Bus>();

        while (buses.Count > 0)
        {
            foreach (var bus in buses)
            {
                bus.CheckWay();
            }

            Bus selectedBus = buses.Find(bus => bus.IsWayClear);

            if (selectedBus != null)
            {
                selectedBus.SetCollider(false);
                sequence.Add(selectedBus);
                buses.Remove(selectedBus);
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

        return sequence;
    }

    private List<Material> GenerateColorSequenceForPeopleHard(List<Bus> buses, int randomDelimeter)
    {
        var colorSequence = new List<Material>();

        if (buses.Count < 2 || randomDelimeter <= 0)
            return colorSequence;

        for (int j = 0; j < buses.Count - 1; j++)
        {
            var currentBus = buses[j];


            if (j + 2 < buses.Count)
            {
                var nextBus = buses[j + 1];
                var next2Bus = buses[j + 2];
                int currentBusPart = currentBus.Capacity / randomDelimeter;
                int nextBusPart = nextBus.Capacity / randomDelimeter;
                int next2BusPart = next2Bus.Capacity / randomDelimeter;

                int currentBusRemainder = currentBus.Capacity - currentBusPart;
                int nextBusRemainder = nextBus.Capacity - nextBusPart;
                int next2BusRemainder = next2Bus.Capacity - next2BusPart;

                for (int i = 0; i < currentBusPart; i++)
                    colorSequence.Add(currentBus.Color);

                for (int i = 0; i < nextBusPart; i++)
                    colorSequence.Add(nextBus.Color);

                for (int i = 0; i < next2BusPart; i++)
                    colorSequence.Add(next2Bus.Color);

                for (int i = 0; i < currentBusRemainder; i++)
                    colorSequence.Add(currentBus.Color);

                for (int i = 0; i < nextBusRemainder; i++)
                    colorSequence.Add(nextBus.Color);

                for (int i = 0; i < next2BusRemainder; i++)
                    colorSequence.Add(next2Bus.Color);

                j += 2;
            }
            else if (j + 1 < buses.Count)
            {
                var nextBus = buses[j + 1];
                int currentBusPart = currentBus.Capacity - randomDelimeter;
                int nextBusPart = nextBus.Capacity - randomDelimeter;

                int currentBusRemainder = currentBus.Capacity - currentBusPart;
                int nextBusRemainder = nextBus.Capacity - nextBusPart;

                for (int i = 0; i < currentBusPart; i++)
                    colorSequence.Add(currentBus.Color);

                for (int i = 0; i < nextBusPart; i++)
                    colorSequence.Add(nextBus.Color);

                for (int i = 0; i < currentBusRemainder; i++)
                    colorSequence.Add(currentBus.Color);

                for (int i = 0; i < nextBusRemainder; i++)
                    colorSequence.Add(nextBus.Color);

                j++;
            }
            else
            {
                for (int i = 0; i < buses[i].Capacity; i++)
                {
                    colorSequence.Add(buses[i].Color);
                }
            }
        }

        var lastBus = buses[^1];
        for (int i = 0; i < lastBus.Capacity; i++)
        {
            colorSequence.Add(lastBus.Color);
        }

        return colorSequence;
    }

    private List<Material> GenerateColorSequenceForPeopleEase(List<Bus> buses, int randomDelimeter)
    {
        var colorSequence = new List<Material>();

        if (buses.Count < 2 || randomDelimeter <= 0)
            return colorSequence;

        for (int j = 0; j < buses.Count - 1; j++)
        {
            var currentBus = buses[j];
            if (j == 0)
            {
                for (int i = 0; i < buses[j].Capacity; i++)
                    colorSequence.Add(currentBus.Color);
            }
            else if (j + 1 < buses.Count)
            {
                var nextBus = buses[j + 1];
                int currentBusPart = currentBus.Capacity - randomDelimeter;
                int nextBusPart = nextBus.Capacity - randomDelimeter;

                int currentBusRemainder = currentBus.Capacity - currentBusPart;
                int nextBusRemainder = nextBus.Capacity - nextBusPart;

                for (int i = 0; i < currentBusPart; i++)
                    colorSequence.Add(currentBus.Color);

                for (int i = 0; i < nextBusPart; i++)
                    colorSequence.Add(nextBus.Color);

                for (int i = 0; i < currentBusRemainder; i++)
                    colorSequence.Add(currentBus.Color);

                for (int i = 0; i < nextBusRemainder; i++)
                    colorSequence.Add(nextBus.Color);

                j++;
            }
            else
            {
                for (int i = 0; i < buses[i].Capacity; i++)
                {
                    colorSequence.Add(buses[i].Color);
                }
            }
        }

        var lastBus = buses[^1];
        for (int i = 0; i < lastBus.Capacity; i++)
        {
            colorSequence.Add(lastBus.Color);
        }

        return colorSequence;
    }

    private void LoadDifficultyData()
    {
        if (MirraSDK.Data.HasKey("Difficulty"))
        {
            _difficulty = (Difficulty)MirraSDK.Data.GetInt("Difficulty");
        }
        else
        {
            _difficulty = Difficulty.Easy;
        }

        //цвета
        int level = _levelLoader.Level;
        bool hasMatch = false;
        for (int i = 0; i < _levelCollection.levels.Count; i++)
        {
            if (level == i)
            {
                hasMatch = true;
                NumberOfColors = _levelCollection.levels[i].ColorsCount;
            }
        }

        if (!hasMatch)
        {
            NumberOfColors = 8;
        }
    }
}