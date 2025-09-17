using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextDisplay : UnitsInteractableUI
{
    void Awake()
    {
        offsetY = 0f;
        offsetYMax = 64f;
    }
    void LateUpdate()
    {
        offsetY = Mathf.Lerp(offsetY, offsetYMax, Time.deltaTime * 5f);
        if (followTarget != null) transform.position = Camera.main.WorldToScreenPoint(followTarget.position) + new Vector3(0, offsetY);
    }
}
