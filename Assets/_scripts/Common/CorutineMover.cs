using System.Collections.Generic;
using MEC;
using UnityEngine;

public class CorutineMover : MonoBehaviour
{
    public float _delay;
    public Vector3 offset = new Vector3(1, 0, 0);
    public float moveSpeed = 1.0f;
    public float returnSpeed = 0.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + offset);
    }

    private void Start()
    {
        Timing.RunCoroutine(MoveAndReturn());
    }

    private IEnumerator<float> MoveAndReturn()
    {
        yield return Timing.WaitForSeconds(_delay);
        while (true)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + offset;

            float journeyLength = Vector3.Distance(startPosition, targetPosition);
            float startTime = Time.time;
            float distCovered = 0;
            float fractionOfJourney = 0;

            while (fractionOfJourney < 1)
            {
                distCovered = (Time.time - startTime) * moveSpeed;
                fractionOfJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
                yield return Timing.WaitForOneFrame;
            }

            startTime = Time.time;
            distCovered = 0;
            fractionOfJourney = 0;

            while (fractionOfJourney < 1)
            {
                distCovered = (Time.time - startTime) * returnSpeed;
                fractionOfJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(targetPosition, startPosition, fractionOfJourney);
                yield return Timing.WaitForOneFrame;
            }
        }
    }
}