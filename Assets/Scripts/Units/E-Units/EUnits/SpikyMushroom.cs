using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikyMushroom : EUnits
{
    protected override void Init()
    {
        maxHP = 2500;
        speed = 1.75f;
        jumpPower = 11f;
        base.Init();
    }
}
