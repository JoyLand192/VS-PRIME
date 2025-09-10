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
    private Camera camComp;
    public Camera CamComp
    {
        get => camComp;
        set => camComp = value;
    }
    private Transform target, cam;
    private Vector3 defaultPosition;
    private Vector3 fixedOffsets;
    private Vector3 destination;
    public float moveSpeed;
    private float xOffset;
    public float XOffset
    {
        get => xOffset;
        set
        {
            yOffset = value;
            UpdateOffset();
        }
    }
    private float yOffset;
    public float YOffset
    {
        get => yOffset;
        set
        {
            yOffset = value;
            UpdateOffset();
        }
    }
    private float xShake;
    public float XShake
    {
        get => xShake;
        set
        {
            xShake = value;
            UpdateOffset();
        }
    }
    private float yShake;
    public float YShake
    {
        get => yShake;
        set
        {
            yShake = value;
            UpdateOffset();
        }
    }
    public Vector2 cameraMin, cameraMax;
    public bool followingTarget = true;
    void UpdateOffset() => fixedOffsets = new Vector3(XOffset + XShake, YOffset + YShake, 0);
    void Awake()
    {
        instance = this;
        camComp = GetComponent<Camera>();
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
        cam.localPosition = new Vector3(defaultPosition.x + fixedOffsets.x, defaultPosition.y + fixedOffsets.y, -20f);
    }
    void MoveToTarget()
    {
        if (target == null) return;

        destination = Vector3.Lerp(cam.localPosition - fixedOffsets, target.localPosition, Time.deltaTime * moveSpeed);
        destination = new Vector3(
            Mathf.Clamp(destination.x, cameraMin.x, cameraMax.x),
            Mathf.Clamp(destination.y, cameraMin.y, cameraMax.y)
        );

        defaultPosition = destination;
    }
    public IEnumerator Scale(float value, float duration, Ease ease = Ease.Linear)
    {
        yield return camComp.DOOrthoSize(value, duration).SetEase(ease);
    }
    public IEnumerator MoveTween(Vector3 destination, float duration, Ease ease = Ease.Linear, bool isAsync = false)
    {
        followingTarget = false;
        LerpPosition(destination, duration, ease);

        if (isAsync) yield return new WaitForSeconds(duration);
        yield return null;
    }

    public void LerpPosition(Vector3 destination, float duration, Ease ease)
    {
        DOTween.To(
            () => defaultPosition,
            (value) => defaultPosition = value,
            destination,
            duration
        ).SetEase(ease);
    }
    public IEnumerator Shake(float xStrength, float yStrength, float duration, bool softness = true)
    {
        StopCoroutine("ShakeCameraAsync");
        yield return StartCoroutine(ShakeCameraAsync(xStrength, yStrength, duration, softness));
    }
    public IEnumerator ShakeCameraAsync(float xStr, float yStr, float duration, bool softness)
    {
        float soft = 1f;
        float t = 0f;

        while (t < duration)
        {
            XShake = Random.Range(-xStr, xStr) * soft;
            YShake = Random.Range(-yStr, yStr) * soft;

            soft = softness ? 1 - t / duration : 1f;
            t += Time.deltaTime;
            yield return null;
        }

        XShake = 0;
        YShake = 0;
    }
    public void ChangeTarget(Transform t)
    {
        target = t;
    }

    public void SetOffset(float? x = null, float? y = null)
    {
        XOffset = x ?? XOffset;
        YOffset = y ?? YOffset;
    }

    public void UpdateCameraCap()
    {
        cameraMin = GameManager.Instance.CamMin;
        cameraMax = GameManager.Instance.CamMax;
    }
}
