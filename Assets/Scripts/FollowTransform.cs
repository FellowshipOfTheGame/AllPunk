using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform target;
    private Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }

    void LateUpdate()
    {
        myTransform.position = target.position;
        myTransform.rotation = target.rotation;
    }
}
