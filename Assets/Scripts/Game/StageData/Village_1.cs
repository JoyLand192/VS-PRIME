using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village_1 : Stage
{
    Action A = delegate ()
    {
        Debug.Log("WOW THE FUCKING SMILE PLATFOMR");
        return;
    };
    Action B = delegate ()
    {
        return;
    };
    void Awake()
    {
        TriggerEvents = new()
        {
            A,
            B,
        };
    }
}
