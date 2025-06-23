using Assets._scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class FollowPath : MonoBehaviour
{
    [SerializeField] private BusGenerator _busGenerator;
    [SerializeField] private ColorManager _colorManager;
    [SerializeField] private PersonLoader _personLoader;
    [SerializeField] private ParkingManager _parkingManager;

    [Header("Настройки движения")] [SerializeField]
    private Person _personPrefab;

    [SerializeField] private Transform[] _pathPoints;
    [SerializeField] private Transform _personsParent;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _followDistance = 1f;
    [SerializeField] private float _spawnDelay = 1f;
    [SerializeField] private float _spawnCheckRadius = 0.5f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private TextMeshProUGUI _personsLeftText;
    [SerializeField] private List<ColorData> _colorSequence;


    public event Action<int> PersonsLeftChanged;

    private int _personStartCount;
    private int _personLeft;
    private List<Person> personsList = new List<Person>();
    private Dictionary<Person, int> personTargetIndex = new Dictionary<Person, int>();
    private int _personIndex = 0;


    public void Initialize()
    {
        _colorSequence = new List<ColorData>(_colorManager.ColorSequencePerson);
        CalcualtePersonsCount();
        StartCoroutine(CreatePersonsCoroutine());
        StartCoroutine(MoveAlongPath());
        _personLeft = _personStartCount;
        _personsLeftText.text = LocalizationManager.Instance.GetText("person_left") + "\n" + _personLeft.ToString();
    }

    public void UpdatePersonsLeftCount()
    {
        _personLeft--;
        if (_personLeft < 0)
            _personLeft = 0;
        _personsLeftText.text = LocalizationManager.Instance.GetText("person_left") + "\n" + _personLeft.ToString();
        PersonsLeftChanged?.Invoke(_personLeft);
    }

    public void DeletePerson(Person personToRemove)
    {
        if (personsList.Contains(personToRemove))
        {
            personsList.Remove(personToRemove);
            //Destroy(personToRemove.gameObject);
        }
    }

    private void CalcualtePersonsCount()
    {
        foreach (var bus in _busGenerator.BusesInFirstArea)
        {
            _personStartCount += bus.Capacity;
        }

        foreach (var bus in _busGenerator.BusesInSecondArea)
        {
            _personStartCount += bus.Capacity;
        }

        foreach (var bus in _busGenerator.BusesInThirdArea)
        {
            _personStartCount += bus.Capacity;
        }
    }

    private IEnumerator CreatePersonsCoroutine()
    {
        if (_personPrefab == null)
        {
            yield break;
        }

        int createdPersons = 0;
        Vector3 spawnPosition = _pathPoints[0].position;

        while (createdPersons < _personStartCount)
        {
            if (!IsPositionOccupied(spawnPosition) && _colorSequence.Count>0)
            {
                Person newPerson = Instantiate(_personPrefab, spawnPosition, Quaternion.identity, _personsParent);
                personsList.Add(newPerson);
                AssignColorToPerson(newPerson, _colorSequence);
                personTargetIndex[newPerson] = 0;
                createdPersons++;
            }

            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, _spawnCheckRadius, _layerMask);
        return colliders.Length > 0;
    }

    // private IEnumerator MoveAlongPath()
    // {
    //     if (_pathPoints == null || _pathPoints.Length == 0)
    //     {
    //         Debug.LogError("Траектория не задана!");
    //         yield break;
    //     }
    //
    //     while (true)
    //     {
    //         for (int i = 0; i < personsList.Count; i++)
    //         {
    //             Person currentPerson = personsList[i];
    //
    //             int targetIndex = personTargetIndex[currentPerson];
    //             Vector3 targetPoint = _pathPoints[targetIndex].position;
    //
    //             if (Vector3.Distance(currentPerson.transform.position, targetPoint) < 0.1f)
    //             {
    //                 if (targetIndex < _pathPoints.Length - 1)
    //                 {
    //                     personTargetIndex[currentPerson]++;
    //                 }
    //                 else
    //                 {
    //                     _personLoader.SetPerson(currentPerson);
    //                     continue;
    //                 }
    //             }
    //
    //             if (i > 0)
    //             {
    //                 Person previousPerson = personsList[i - 1];
    //                 if (Vector3.Distance(currentPerson.transform.position, previousPerson.transform.position) <=
    //                     _followDistance)
    //                 {
    //                     // currentPerson.SetRunningAnimation(false);
    //                     continue;
    //                 }
    //             }
    //
    //             currentPerson.transform.position =
    //                 Vector3.MoveTowards(currentPerson.transform.position, targetPoint, _speed * Time.deltaTime);
    //             // currentPerson.SetRunningAnimation(true);
    //         }
    //
    //         yield return null;
    //     }
    // }

    private IEnumerator MoveAlongPath()
    {
        if (_pathPoints == null || _pathPoints.Length == 0)
        {
            Debug.LogError("Траектория не задана!");
            yield break;
        }
        float moveStep = _speed * Time.fixedDeltaTime;

        while (true)
        {
            for (int i = 0; i < personsList.Count; i++)
            {
                Person currentPerson = personsList[i];
                int targetIndex = personTargetIndex[currentPerson];
                Vector3 targetPoint = _pathPoints[targetIndex].position;

                if (Vector3.Distance(currentPerson.transform.position, targetPoint) < moveStep)
                {
                    currentPerson.transform.position = targetPoint;

                    if (targetIndex < _pathPoints.Length - 1)
                    {
                        personTargetIndex[currentPerson]++;
                    }
                    else
                    {
                        _personLoader.SetPerson(currentPerson);
                        continue;
                    }
                }
                else
                {
                    if (i > 0)
                    {
                        Person previousPerson = personsList[i - 1];
                        if (Vector3.Distance(currentPerson.transform.position, previousPerson.transform.position) <=
                            _followDistance)
                        {
                            continue;
                        }
                    }

                    Vector3 direction = (targetPoint - currentPerson.transform.position).normalized;
                    currentPerson.transform.position += direction * moveStep;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        if (_pathPoints != null && _pathPoints.Length > 1)
        {
            for (int i = 0; i < _pathPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(_pathPoints[i].position, _pathPoints[i + 1].position);
            }
        }
    }

    private void AssignColorToPerson(Person person, List<ColorData> colorSequence)
    {
        //if (_personIndex < 0 || _personIndex >= colorSequence.Count)
        // {
        //     Debug.LogWarning($"Index {_personIndex} is out of range for the color sequence.");
        //     return;
        // }
        // var color = colorSequence[_personIndex];

        var color = colorSequence[0];


        person.Color = color.peopleColor;
        person.ColorType = color.colorType;
        person.meshRenderer.material = color.peopleColor;
        _personIndex++;
        colorSequence.RemoveAt(0);
    }

//     [ContextMenu("ReorderPersonsAccordingBusInSpot")]
//     public void ReorderPersonsAccordingBusInSpot()
// {
//     List<Material> busMaterials = new List<Material>();
//     foreach (var bus in _parkingManager.BusesInSpot)
//     {
//         int count = bus.Capacity - bus.PersonsInside;
//         for (int i = 0; i < count; i++)
//         {
//             if (busMaterials.Count < personsList.Count)
//                 busMaterials.Add(bus.Color);
//         }
//     }
//     
//     while (busMaterials.Count < personsList.Count)
//     {
//         busMaterials.Add(personsList[busMaterials.Count].Color);
//     }
//     
//     Debug.Log("цикл по добавлению busMaterials " + busMaterials.Count);
//
//     List<Material> replacedMaterials = new List<Material>();
//
//     for (int i = 0; i < personsList.Count; i++)
//     {
//         if (i < busMaterials.Count)
//         {
//             replacedMaterials.Add(personsList[i].Color);
//             personsList[i].Color = busMaterials[i];
//         }
//     }
//     Debug.Log("цикл по добавлению replacedMaterials " + busMaterials.Count + " " + replacedMaterials.Count);
//
//     for (int i = busMaterials.Count - 1; i >= 0; i--)
//     {
//         if (replacedMaterials.Contains(busMaterials[i]))
//         {
//             replacedMaterials.Remove(busMaterials[i]);
//             busMaterials.RemoveAt(i);
//         }
//     }
//     Debug.Log("цикл по удалению совпадений завершен " + busMaterials.Count + " " + replacedMaterials.Count);
//
//     for (int i = 0; i < _colorSequence.Count; i++)
//     {
//         if (busMaterials.Count > 0 && replacedMaterials.Count > 0)
//         {
//             if (busMaterials[0] == _colorSequence[i])
//             {
//                 _colorSequence[i] = replacedMaterials[0];
//                 replacedMaterials.RemoveAt(0);
//                 busMaterials.RemoveAt(0);
//             }
//         }
//     }
//     Debug.Log("цикл по удалению в очереди людей " + busMaterials.Count + " " + replacedMaterials.Count);
// }

    public void ReorderPersonsAccordingBusInSpot()
    {
        List<ColorType> busMaterials = new List<ColorType>();

        foreach (var bus in _parkingManager.BusesInSpot)
        {
            int count = bus.Capacity - bus.PersonsInside;
            for (int i = 0; i < count; i++)
            {
                if (busMaterials.Count < personsList.Count + _colorSequence.Count)
                    busMaterials.Add(bus.ColorType);
            }
        }

        List<ColorType> allColors = new List<ColorType>();
        allColors.AddRange(personsList.Select(person => person.ColorType));
        foreach (var color in _colorSequence)
        {
            allColors.Add(color.colorType);
        }
        //allColors.AddRange(_colorSequence);

        foreach (var color in busMaterials)
        {
            var index = allColors.IndexOf(color);
            if (index != -1)
            {
                allColors.RemoveAt(index);
            }
        }

        List<ColorType> reorderedColors = new List<ColorType>(busMaterials);
        reorderedColors.AddRange(allColors);

        int personCount = personsList.Count;
        for (int i = 0; i < reorderedColors.Count; i++)
        {
            if (i < personCount)
            {
                personsList[i].Color = _colorManager.GetPeopleColor(reorderedColors[i]);
                personsList[i].ColorType = reorderedColors[i];
            }
            else if (i - personCount < _colorSequence.Count)
            {
                _colorSequence[i - personCount].peopleColor = _colorManager.GetPeopleColor(reorderedColors[i]);
                _colorSequence[i - personCount].colorType = reorderedColors[i];
            }
        }
    }
}