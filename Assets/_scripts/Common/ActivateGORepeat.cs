using System.Collections;
using UnityEngine;

public class ActivateGORepeat : MonoBehaviour
{
    public GameObject[] objectsToActivate;
    public float activeTime = 2f; 
    public float inactiveTime = 2f;
    public bool _isRandomActivatedTime = true;

    private void Start()
    {
        
        StartCoroutine(ActivateObjectsCoroutine());
    }

    private IEnumerator ActivateObjectsCoroutine()
    {
        if (_isRandomActivatedTime)
        {
            float time = Random.Range(0f, activeTime);
            yield return new WaitForSeconds(time);
        }
        while (true)
        {
            foreach (var obj in objectsToActivate)
            {
                obj.SetActive(true);
            }

            yield return new WaitForSeconds(activeTime);

            foreach (var obj in objectsToActivate)
            {
                obj.SetActive(false);
            }

            yield return new WaitForSeconds(inactiveTime);
        }
    }
}
