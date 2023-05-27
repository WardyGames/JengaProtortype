using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher Instance;

    void Awake()
    {
        Instance = this;
    }

    public void GoToNewStackTower(Transform newTarget)
    {
        Vector3 newPosition = transform.position;
        newPosition.x = newTarget.position.x;
        transform.position = newPosition;
    }
}
