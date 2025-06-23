using System.Collections.Generic;
using System.Linq;
using _scripts.Settings;
using UnityEditor;
using UnityEngine;
using MirraGames.SDK;
using MirraGames.SDK.Common;
using Assets._scripts;

public enum Difficulty
{
    Easy = 0,
    Hard = 1
}

[SelectionBase]
public class ColorManager : MonoBehaviour
{
    [SerializeField] private FollowPath _followPath;
    [SerializeField] private LevelCollection _levelCollection;
    [SerializeField] private LevelLoader _levelLoader;
    [Range(1, 8)] public int NumberOfColors = 8;
    [Range(5, 100)] public int ZoneMixPercentage = 10;

    public List<ColorData> GeneratedColors;
    public List<Bus> BusSequence;
    public List<ColorData> ColorSequencePerson;

    private List<ColorData> _firstZoneColors;
    private List<ColorData> _secondZoneColors;
    private List<ColorData> _thirdZoneColors;
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
                Debug.Log("ColorSequencePerson count: " + ColorSequencePerson.Count);
                break;
            case Difficulty.Hard:
                ColorSequencePerson = GenerateColorSequenceForPeopleHard(BusSequence, _randomDelimeter);
                Debug.Log("ColorSequencePerson count: " + ColorSequencePerson.Count);
                break;
        }

        _followPath.Initialize();
        //AssignHidedBuses();
    }

    public Material GetPeopleColor(ColorType color)
    {
        foreach (var col in GeneratedColors)
        {
            if (col.colorType == color)
            {
                return col.peopleColor;
            }
        }
        return null;
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

    private void AssignBusColors(List<Bus> buses, List<ColorData> zoneColors)
    {
        foreach (var bus in buses)
        {
            // Выбираем случайный цвет из доступных цветов зоны
            int colorIndexInZone = Random.Range(0, zoneColors.Count);
            ColorData selectedColor = zoneColors[colorIndexInZone];

            bus.Color = selectedColor.GetColor(bus.Type);
            bus.ColorType=selectedColor.colorType;
            // bus.meshRendererBody.materials[0].color = selectedColor;
            // bus.meshRendererTop.materials[0].color = selectedColor;
            bus.meshRendererBody.material = selectedColor.GetColor(bus.Type);
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

    private List<ColorData> GenerateColorSequenceForPeopleHard(List<Bus> buses, int randomDelimeter)
    {
        var colorSequence = new List<ColorData>();

        if (buses.Count < 2 || randomDelimeter <= 0)
            return colorSequence;

        for (int j = 0; j < buses.Count; j++)
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
                {
                    var color = GeneratedColors.Where(color => color.GetColor(currentBus.Type) == currentBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType,currentBus.Type, currentBus.Color, color.peopleColor));
                }

                for (int i = 0; i < nextBusPart; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(nextBus.Type) == nextBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType, currentBus.Type, nextBus.Color, color.peopleColor));
                }

                for (int i = 0; i < next2BusPart; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(next2Bus.Type) == next2Bus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType, currentBus.Type, next2Bus.Color, color.peopleColor));
                }

                for (int i = 0; i < currentBusRemainder; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(currentBus.Type) == currentBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType, currentBus.Type, currentBus.Color, color.peopleColor));
                }

                for (int i = 0; i < nextBusRemainder; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(nextBus.Type) == nextBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType, currentBus.Type, nextBus.Color, color.peopleColor));
                }

                for (int i = 0; i < next2BusRemainder; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(next2Bus.Type) == next2Bus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType, currentBus.Type, next2Bus.Color, color.peopleColor));
                }

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
                {
                    var color = GeneratedColors.Where(color => color.GetColor(currentBus.Type) == currentBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType, currentBus.Type, currentBus.Color, color.peopleColor));
                }

                for (int i = 0; i < nextBusPart; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(nextBus.Type) == nextBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType, nextBus.Type, nextBus.Color, color.peopleColor));
                }

                for (int i = 0; i < currentBusRemainder; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(currentBus.Type) == currentBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType, currentBus.Type, currentBus.Color, color.peopleColor));
                }

                for (int i = 0; i < nextBusRemainder; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(nextBus.Type) == nextBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType, nextBus.Type, nextBus.Color, color.peopleColor));
                }

                j++;
            }
            else
            {
                for (int i = 0; i < buses[j].Capacity; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(buses[j].Type) == buses[j].Color).First<ColorData>();
                    colorSequence.Add((new ColorData(color.colorType, buses[j].Type, buses[j].Color, color.peopleColor)));
                }
            }
        }

       /* var lastBus = buses[^1];
        for (int i = 0; i < lastBus.Capacity; i++)
        {
            var color = GeneratedColors.Where(color => color.GetColor(lastBus.Type) == lastBus.Color).First<ColorData>();
            colorSequence.Add(new ColorData(color.colorType, lastBus.Type, lastBus.Color, color.peopleColor));
        }*/

        return colorSequence;
    }

    private List<ColorData> GenerateColorSequenceForPeopleEase(List<Bus> buses, int randomDelimeter)
    {
        var colorSequence = new List<ColorData>();

        if (buses.Count < 2 || randomDelimeter <= 0)
            return colorSequence;

        for (int j = 0; j < buses.Count; j++)
        {
            var currentBus = buses[j];
            if (j == 0)
            {
                for (int i = 0; i < buses[j].Capacity; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(currentBus.Type) == currentBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType,currentBus.Type, currentBus.Color,color.peopleColor));
                }
            }
            else if (j + 1 < buses.Count)
            {
                var nextBus = buses[j + 1];
                int currentBusPart = currentBus.Capacity - randomDelimeter;
                int nextBusPart = nextBus.Capacity - randomDelimeter;

                int currentBusRemainder = currentBus.Capacity - currentBusPart;
                int nextBusRemainder = nextBus.Capacity - nextBusPart;

                for (int i = 0; i < currentBusPart; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(currentBus.Type) == currentBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType,currentBus.Type, currentBus.Color, color.peopleColor));
                }

                for (int i = 0; i < nextBusPart; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(nextBus.Type) == nextBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType,nextBus.Type, nextBus.Color, color.peopleColor));
                }

                for (int i = 0; i < currentBusRemainder; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(currentBus.Type) == currentBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType,currentBus.Type, currentBus.Color, color.peopleColor));
                }

                for (int i = 0; i < nextBusRemainder; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(nextBus.Type) == nextBus.Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType,nextBus.Type, nextBus.Color, color.peopleColor));
                }

                j++;
            }
            else
            {
                for (int i = 0; i < buses[j].Capacity; i++)
                {
                    var color = GeneratedColors.Where(color => color.GetColor(buses[j].Type) == buses[j].Color).First<ColorData>();
                    colorSequence.Add(new ColorData(color.colorType,buses[j].Type, buses[j].Color, color.peopleColor));
                }
            }
        }

        /*var lastBus = buses[^1];
        for (int i = 0; i < lastBus.Capacity; i++)
        {
            var color = GeneratedColors.Where(color => color.GetColor(lastBus.Type) == lastBus.Color).First<ColorData>();
            colorSequence.Add(new ColorData(color.colorType,lastBus.Type, lastBus.Color, color.peopleColor));
        }*/

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