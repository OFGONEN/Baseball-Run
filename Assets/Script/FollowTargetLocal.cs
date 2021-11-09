using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetLocal : MonoBehaviour
{
    public Transform target;

    private Vector3 diff;

    private void Awake()
    {
        diff = target.localPosition - transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = target.localPosition - diff;
    }
}
