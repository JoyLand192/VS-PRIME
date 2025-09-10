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
        };
    }
    IEnumerator Event1()
    {
        ToggleCrMove(false);
        ToggleHUD(0f, 0.4f);

        yield return Wait(1f);

        Debug.Log("CR이 위를 바라보는 연출");
        ToggleCrTrigger("LookUp");

        yield return Wait(1.4f);

        yield return MoveCamera(new Vector2(22, 14), 2f, "OutCirc");

        yield return ScaleCamera(7, 0.9f, "OutCirc");

        yield return Wait(3f);

        ToggleCameraFollow(true);

        yield return Wait(1.3f);

        ToggleCrTrigger("StopLookUp");

        yield return Wait(0.7f);

        Debug.Log("대화창 : wow");
        ToggleHUD(1f, 0.4f);

        yield return Wait(0.5f);

        ToggleCrMove(true);
    }
    IEnumerator Event2()
    {
        yield return null;
    }
}
