using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : EUnits
{
    protected override void Init()
    {
        maxHP = 99999;
        speed = 0f;
        jumpPower = 0f;
        base.Init();
    }
}
