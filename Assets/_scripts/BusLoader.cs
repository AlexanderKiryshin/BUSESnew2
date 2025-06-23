using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _scripts
{
    public class BusLoader: MonoBehaviour
    {
        [SerializeField] private BusGenerator _busGenerator;
        [SerializeField] private  Bus smallBusPrefab;
        [SerializeField] private  Bus mediumBusPrefab;
        [SerializeField] private  Bus largeBusPrefab;

        public void LoadBusData(BusData busData)
        {
            foreach (var busInfo in busData.buses)
            {
                Bus busPrefab = null;
                switch (busInfo.busType)
                {
                    case BusType.Small:
                        busPrefab = smallBusPrefab;
                        break;
                    case BusType.Medium:
                        busPrefab = mediumBusPrefab;
                        break;
                    case BusType.Large:
                        busPrefab = largeBusPrefab;
                        break;
                }

                if (busPrefab != null)
                {
                    Bus bus = Instantiate(busPrefab, busInfo.position, busInfo.rotation);
                    bus.Type = busInfo.busType;
                    _busGenerator.AllBuses.Add(bus);
                    _busGenerator.BusesInFirstArea.Add(bus);
                }
            }
        }

        public void SaveBusData(BusData busData)
        {
            GameObject[] buses = GameObject.FindGameObjectsWithTag("Bus");
            busData.buses = new BusPositionAsset[buses.Length];

            for (int i = 0; i < buses.Length; i++)
            {
                BusPositionAsset busInfo = new BusPositionAsset
                {
                    position = buses[i].transform.position,
                    rotation = buses[i].transform.rotation,
                    busType = buses[i].GetComponent<Bus>().Type
                };
                busData.buses[i] = busInfo;
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(busData);
#endif
        }
    }
}