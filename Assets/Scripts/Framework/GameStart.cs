using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;

    private void Awake()
    {
        AppConst.GameMode = GameMode;
    }
}
