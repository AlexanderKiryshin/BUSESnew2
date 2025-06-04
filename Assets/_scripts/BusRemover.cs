using System;
using UnityEngine;

public class BusRemover : MonoBehaviour
{
    public event Action OnBusDestroy;
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Bus"))
        {
            OnBusDestroy?.Invoke();
            Destroy(other.gameObject);
           
        }
    }
}
