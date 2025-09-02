using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGElement : MonoBehaviour
{
    [Range(0, 1f)] public float xParallaxFactor;
    [Range(0, 1f)] public float yParallaxFactor; // 0 = 0%, 1 = 100%
    public float xOffset, yOffset;
    Camera cam;
    Vector2 CV, Origin;

    void Awake()
    {
        Origin = transform.localPosition;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void LateUpdate()
    {
        CV = cam.transform.position;
        transform.position = Origin + new Vector2(CV.x * xParallaxFactor + xOffset, CV.y * yParallaxFactor + yOffset);
    }
}
