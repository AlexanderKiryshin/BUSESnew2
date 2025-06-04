using System.Collections.Generic;
using UnityEngine;

namespace _scripts.Settings
{
    [CreateAssetMenu (fileName = "LevelCollection", menuName = "Bus/LevelCollection")]
    public class LevelCollection: ScriptableObject
    {
        public List<LevelAsset> levels;

        private void OnValidate()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].ColorsCount > 8 || levels[i].ColorsCount < 1)
                    levels[i].ColorsCount = 1;
                if (levels[i].busData != null)
                {
                    levels[i].SmallBusCount = 0;
                    levels[i].MediumBusCount = 0;
                    levels[i].LargeBusCount = 0;
                }
            }
            
        }
    }
    
}