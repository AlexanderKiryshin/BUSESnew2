using UnityEngine;

namespace _Scripts.Common
{
    public class ParentOnTriggerEnter: MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent = transform;
            }
        }
    }
}