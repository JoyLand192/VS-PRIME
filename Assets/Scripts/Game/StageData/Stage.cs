using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public int triggerBitMask;
    public StageData data;
    public List<IEnumerator> TriggerEvents = new();
    public void ToggleHUD(float value, float duration) => GameManager.Instance.setHudVisible(value, duration);
    public void ToggleCrMove(bool value) => GameManager.Instance.cr.SetMove(value);
    public void ToggleCrTrigger(string name, bool? value = null) => GameManager.Instance.cr.SetAnimTrigger(name, value);
    public void ToggleCameraFollow(bool value) => DefaultCamera.Instance.followingTarget = value;
    public IEnumerator ScaleCamera(float value, float duration, string ease = "Linear")
    {
        yield return DefaultCamera.Instance.Scale(value, duration, Enum.TryParse<DG.Tweening.Ease>(ease, true, out var result) ? result : DG.Tweening.Ease.Linear);
    }
    public IEnumerator ShakeCamera(float xStr, float yStr, float duration, bool softness = true)
    {
        yield return DefaultCamera.Instance.Shake(xStr, yStr, duration, softness);
    }
    public IEnumerator MoveCamera(Vector2 destination, float duration, string ease = "Linear", bool isAsync = true)
    {
        yield return DefaultCamera.Instance.MoveTween((Vector3)destination, duration, Enum.TryParse<DG.Tweening.Ease>(ease, true, out var result) ? result : DG.Tweening.Ease.Linear, isAsync);
    }
    public IEnumerator Wait(float value)
    {
        yield return new WaitForSeconds(value);
    }
}
