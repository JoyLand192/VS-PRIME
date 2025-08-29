using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MenuButton : HoverButton
{
    protected override void Init()
    {
        base.Init();
        tweenDistance = 62f;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        rect.DOKill(true);
        rect.DOAnchorPosX(rect.anchoredPosition.x - tweenDistance, 0.4f).SetEase(Ease.OutCirc).SetUpdate(true);
        rect.DOScale(new Vector2(1.2f, 1.2f), 0.4f).SetEase(Ease.OutExpo).SetUpdate(true);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        rect.DOKill(true);
        rect.DOAnchorPosX(rect.anchoredPosition.x + tweenDistance, 0.4f).SetEase(Ease.OutCirc).SetUpdate(true);
        rect.DOScale(Vector2.one, 0.4f).SetEase(Ease.OutExpo).SetUpdate(true);
    }
}
