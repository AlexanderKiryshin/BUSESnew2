using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class PersonLoader : MonoBehaviour
{
    [SerializeField] private FollowPath _path;
    [SerializeField] private ParkingManager _parkingManager;

    private Person _personInZone;
    private Coroutine _coroutine;

    public event Action<Person> FirstPersonChancged;
    public Coroutine CoroutineFreeSpot => _coroutine;

    public void Update()
    {
        if (_parkingManager.BusesInSpot.Count != 0)
        {
            foreach (var bus in _parkingManager.BusesInSpot)
            {
                if (bus.PersonsInside == bus.Capacity)
                {
                    if (_coroutine == null)
                        _coroutine = StartCoroutine(CallFreeSpotWithDelay(bus, 0.4f));
                    break;
                }

                if (_personInZone == null)
                    return;
                if (bus.ColorType == _personInZone.ColorType)
                {
                    _path.UpdatePersonsLeftCount();
                    bus.TakePerson(_personInZone);
                    SoundManager.instance.PlayPersonLoadSound();
                    DeletePerson();
                    break;
                }
            }
        }
    }

    public void DeletePerson()
    {
        if (_personInZone != null)
        {
            _path.DeletePerson(_personInZone);
            _personInZone = null;
        }
    }

    public void SetPerson(Person person)
    {
        if (person != _personInZone)
        {
            _personInZone = person;
            FirstPersonChancged?.Invoke(_personInZone);
        }
    }
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Person"))
    //     {
    //         _personInZone = other.GetComponent<Person>();
    //         FirstPersonChancged?.Invoke(_personInZone);
    //     }
    // }

    private IEnumerator CallFreeSpotWithDelay(Bus bus, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!bus.isVip)
            _parkingManager.FreeSpot(bus);
        else
            _parkingManager.FreeVipSpot(bus);

        _coroutine = null;
    }
}