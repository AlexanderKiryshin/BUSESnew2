using UnityEngine;

namespace _Scripts.Common
{
    public class ClearParentOnTriggerExit : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent = null;
            }
        }
    }
}