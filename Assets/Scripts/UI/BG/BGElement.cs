using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGElement : MonoBehaviour
{
    public float XparallaxFactor, YparallaxFactor; // 0 = 0%, 1 = 100%
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
        transform.position = Origin + new Vector2(CV.x * XparallaxFactor, CV.y * YparallaxFactor);
    }
}
