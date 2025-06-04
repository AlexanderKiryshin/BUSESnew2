using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [SerializeField] private float _speed = 40f;
    [SerializeField] private float _rotationSpeed = 20f;


    public bool IsFlyingToTarget(Vector3 target)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target);
        if (distanceToTarget < 0.1f)
        {
            return false;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * _speed);
        Vector3 directionToTarget = (target - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

        return true;
    }
}


