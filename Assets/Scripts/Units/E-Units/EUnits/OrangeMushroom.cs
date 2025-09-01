using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeMushroom : EUnits
{
    protected override void Init()
    {
        maxHP = 500;
        speed = 3.75f;
        jumpPower = 13f;
        base.Init();
    }
}
