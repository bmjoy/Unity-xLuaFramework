using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static ResourceManager Resource { get; private set; }

    private void Awake()
    {
        Resource = gameObject.AddComponent<ResourceManager>();
    }
}
