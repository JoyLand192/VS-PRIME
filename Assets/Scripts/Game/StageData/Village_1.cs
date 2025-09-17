using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village_1 : Stage
{
    void Awake()
    {
        TriggerEvents = new()
        {
            Event1(),
            Event2(),
            Event3(),
        };
    }
    IEnumerator Event1()
    {
        ToggleCrMove(false);
        ToggleHUD(0f, 0.4f);

        yield return Wait(1f);

        ToggleCrTrigger("LookUp");

        yield return Wait(1.4f);

        yield return MoveCamera(new Vector2(22, 14), 2f, "InOutCubic");

        yield return ScaleCamera(8.4f, 0.6f, "OutCirc");

        yield return Wait(2f);

        ToggleCameraFollow(true);

        yield return Wait(1.3f);

        ToggleCrTrigger("StopLookUp");

        yield return Wait(0.7f);

        Debug.Log("대화창 : wow");
        ToggleHUD(1f, 0.4f);

        yield return Wait(0.5f);

        ToggleCrMove(true);
        yield return ScaleCamera(10f, 0.8f, "OutCirc");
    }
    IEnumerator Event2()
    {
        ToggleCrMove(false);
        ToggleHUD(0f, 0.4f);

        yield return Wait(0.8f);

        var crPos = GameManager.Instance.cr.transform.position;

        yield return MoveCamera((Vector2)crPos + new Vector2(15, 0), 1.2f, "InExpo");
        yield return MoveCamera((Vector2)crPos + new Vector2(-15, 0), 0f);
        yield return MoveCamera(crPos, 1.2f, "OutExpo");

        ToggleCrMove(true);
        ToggleHUD(1f, 0.4f);
        ToggleCameraFollow(true);
        yield return null;
    }
    IEnumerator Event3()
    {
        ToggleCrTrigger("ULTIMATE");
        yield break;
    }
}
