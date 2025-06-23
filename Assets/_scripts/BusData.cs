using System.Collections.Generic;
using UnityEngine;

namespace _scripts
{
    [CreateAssetMenu(fileName = "BusData", menuName = "Bus/BusData", order = 1)]
    public class BusData: ScriptableObject
    {
        public BusPositionAsset[] buses;
    }
}