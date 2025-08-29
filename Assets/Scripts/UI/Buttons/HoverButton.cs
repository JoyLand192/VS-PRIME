using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float tweenScale = 1.3f;
    public float tweenLength = 0.5f;
    public Vector3 origin;
    public Vector3 originScale;
    protected RectTransform rect;
    public RectTransform RTransform
    {
        get
        {
            if (rect == null) rect = GetComponent<RectTransform>();
            return rect;
        }
    }
    protected float tweenDistance;
    void Awake()
    {
        Init();
    }
    protected virtual void Init()
    {
        rect = GetComponent<RectTransform>();
        origin = rect.anchoredPosition;
        originScale = rect.localScale;
    }
    public void Reset()
    {
        RTransform.anchoredPosition = origin;
        RTransform.localScale = originScale;
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        RTransform.DOKill(true);
        RTransform.DOScale(originScale * tweenScale, tweenLength).SetEase(Ease.OutBounce);
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        RTransform.DOKill(true);
        RTransform.DOScale(originScale * 1f, tweenLength).SetEase(Ease.OutBounce);
    }
}
