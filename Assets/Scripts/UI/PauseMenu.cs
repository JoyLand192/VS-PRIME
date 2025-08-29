using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    float tweenDistance = 110;
    public GameObject ty;
    public RectTransform Menu;
    public RectTransform[] Buttons;
    public RectTransform[] ButtonTexts;
    public void Start()
    {
        foreach (var b in ButtonTexts)
        {
            b.DOAnchorPosX(b.anchoredPosition.x + tweenDistance, 0);
        }
        Menu.gameObject.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
    public void Pause()
    {
        Menu.gameObject.SetActive(true);
        ty.SetActive(true);
        StartCoroutine(PauseAnimation());
    }
    IEnumerator PauseAnimation()
    {
        foreach (var b in ButtonTexts)
        {
            b.DOAnchorPosX(b.anchoredPosition.x - tweenDistance, 0.5f).SetEase(Ease.OutCirc).SetUpdate(true);
            yield return new WaitForSecondsRealtime(0.03f);
        }
    }
    public void Resume()
    {
        foreach (var b in ButtonTexts)
        {
            b.DOKill(true);
            b.anchoredPosition = new Vector2(b.anchoredPosition.x + tweenDistance, b.anchoredPosition.y);
        }

        foreach (var b in Buttons)
        {
            b.DOKill(true);
            b.GetComponent<MenuButton>().Reset();
        }
        Menu.gameObject.SetActive(false);
    }
}
