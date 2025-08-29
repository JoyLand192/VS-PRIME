using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsInteractableUI : MonoBehaviour
{
    public Transform followTarget;
    protected float offsetY;
    protected float offsetYMax;
    public void End()
    {
        Destroy(gameObject);
    }
}
