using UnityEngine;

namespace _Scripts.Common
{
    public class PreventRotation : MonoBehaviour
    {
        private Quaternion initialRotation;

        private void Start()
        {

            initialRotation = transform.rotation;
        }

        private void LateUpdate()
        {
            transform.rotation = initialRotation;
        }
    }
}