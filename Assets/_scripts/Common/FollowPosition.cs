using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = Vector3.zero;

    void Start()
    {
        if (target != null)
            offset = transform.position - target.position;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}