using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public int triggerBitMask;
    public StageData data;
    public List<Action> TriggerEvents = new();
}
