using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static ResourceManager Resource { get; private set; }
    public static LuaManager Lua { get; private set; }

    private void Awake()
    {
        Resource = gameObject.AddComponent<ResourceManager>();
        Lua = gameObject.AddComponent<LuaManager>();
    }
}
