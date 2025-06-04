using System;
using UnityEngine;

public class CoinMover : MonoBehaviour
{
    public float moveDuration = 2f;
    public float curveHeight = 100f;

    private Vector3 startPoint;
    private Vector3 controlPoint;
    private Vector3 endPoint;
    private float elapsedTime;
    private bool isMoving = false;

    public event Action<GameObject> OnReachedTarget; 

    public void MoveToTarget(Vector3 target)
    {
        startPoint = transform.position;
        endPoint = target;

        Vector3 midPoint = (startPoint + endPoint) / 2;
        controlPoint = new Vector3(midPoint.x, midPoint.y + curveHeight, midPoint.z);

        elapsedTime = 0f;
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            Vector3 bezierPosition = Mathf.Pow(1 - t, 2) * startPoint +
                                     2 * (1 - t) * t * controlPoint +
                                     Mathf.Pow(t, 2) * endPoint;

            transform.position = bezierPosition;

            if (t >= 1f)
            {
                isMoving = false;
                OnReachedTarget?.Invoke(gameObject);
            }
        }
    }
}
