using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DefaultCamera : MonoBehaviour
{
    private static DefaultCamera instance;
    public static DefaultCamera Instance
    {
        get
        {
            return instance;
        }
    }
    Transform target, cam;
    Vector3 offsets;
    Vector3 destination;
    public float moveSpeed;
    public float xOffset, yOffset;
    public Vector2 cameraMin, cameraMax;
    public bool followingTarget = true;
    void UpdateOffset() => offsets = new Vector3(xOffset, yOffset, 0);
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        cam = transform;
        target = GameObject.Find("CR").transform;
        moveSpeed = 3f;

        UpdateOffset();
        UpdateCameraCap();
    }
    void Update()
    {
        if (followingTarget) MoveToTarget();
    }
    void MoveToTarget()
    {
        if (target == null) return;

        destination = Vector3.Lerp(cam.localPosition - offsets, target.localPosition, Time.deltaTime * moveSpeed);
        destination = new Vector3(
            Mathf.Clamp(destination.x, cameraMin.x, cameraMax.x),
            Mathf.Clamp(destination.y, cameraMin.y, cameraMax.y)
        );

        cam.localPosition = new Vector3(destination.x + offsets.x, destination.y + offsets.y, -20f);
    }
    public IEnumerator MoveTween(Vector3 destination, float distance, Ease ease = Ease.Linear, bool isAsync = false)
    {
        followingTarget = false;
        var T_tween = cam.DOMove(destination + new Vector3(0, 0, cam.position.z), distance).SetEase(ease);

        if (isAsync) yield return T_tween.Play().WaitForCompletion();
        else T_tween.Play();

        yield return null;
    }
    public void ChangeTarget(Transform t)
    {
        target = t;
    }

    public void SetOffset(float x, float y)
    {
        xOffset = x;
        yOffset = y;
        UpdateOffset();
    }

    public void SetXOffset(float x)
    {
        xOffset = x;
        UpdateOffset();
    }

    public void SetYOffset(float y)
    {
        yOffset = y;
        UpdateOffset();
    }

    public void UpdateCameraCap()
    {
        cameraMin = GameManager.Instance.CamMin;
        cameraMax = GameManager.Instance.CamMax;
    }
}
