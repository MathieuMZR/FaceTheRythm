using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoDestroyEffect : MonoBehaviour
{
    public float destroyDelay;

    private void Start()
    {
        Destroy(gameObject, destroyDelay);
    }
}
