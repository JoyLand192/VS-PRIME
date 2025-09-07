using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trigger : MonoBehaviour
{
    public bool CanRepeat;
    public bool RequiresPreviousEvent;
    public bool SwitchBitMask;
    public int[] RequiredEventNumbers; 
    public int EventNumber;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("CR")) return;
        GameManager.Instance.CastTriggerEvent();
        if (!CanRepeat) Destroy(gameObject);
    }
}
