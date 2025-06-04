
using UnityEditor;
using UnityEngine;

namespace _scripts
{
    public class BusSaver: MonoBehaviour
    {
        

        public BusData busData;
        
#if UNITY_EDITOR
        [ContextMenu("Save")]
        public void SaveBusData()
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

            EditorUtility.SetDirty(busData);
        }
#endif
    }
}
