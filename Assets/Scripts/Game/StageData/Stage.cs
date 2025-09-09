using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public int triggerBitMask;
    public StageData data;
    public List<IEnumerator> TriggerEvents = new();
    public void ToggleHUD(float value, float distance) => GameManager.Instance.setHudVisible(value, distance);
    public void ToggleCameraFollow(bool value) => DefaultCamera.Instance.followingTarget = value;
    public void ToggleCrMove(bool value) => GameManager.Instance.cr.SetMove(value);
    public void ToggleCrTrigger(string name, bool? value = null) => GameManager.Instance.cr.SetAnimTrigger(name, value);

    public IEnumerator MoveCamera(Vector2 destination, float distance, string ease = "Linear", bool isAsync = false)
    {
        yield return DefaultCamera.Instance.MoveTween((Vector3)destination, distance, Enum.TryParse<DG.Tweening.Ease>(ease, out var result) ? result : DG.Tweening.Ease.Linear, isAsync);
    }
    public IEnumerator Wait(float value)
    {
        yield return new WaitForSeconds(value);
    }
}
