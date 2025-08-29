using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefCAMERA : MonoBehaviour
{
    private static DefCAMERA instance; 
    public static DefCAMERA Instance
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
    public bool isStopped = false;
    public bool setToPosition = false;
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
        if (target == null || isStopped) return;

        if (setToPosition) return;
        else
        {
            destination = Vector3.Lerp(cam.localPosition - offsets, target.localPosition, Time.deltaTime * moveSpeed);
            destination = new Vector3(
                Mathf.Clamp(destination.x, cameraMin.x, cameraMax.x),
                Mathf.Clamp(destination.y, cameraMin.y, cameraMax.y)
            );
        }
        cam.localPosition = new Vector3(destination.x + offsets.x, destination.y + offsets.y, -20f);
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
