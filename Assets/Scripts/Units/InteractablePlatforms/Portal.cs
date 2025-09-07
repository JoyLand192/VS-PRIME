using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

public class Portal : MonoBehaviour
{
    SpriteRenderer portalRender;
    float tweenDistance = 2560f;
    bool crTriggered = false;
    bool entering = false;
    public Stage stageData;
    public GameObject localSwipePrefab;
    void Awake()
    {
        portalRender = GetComponent<SpriteRenderer>();
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer != 11) return;
        else crTriggered = true;
        portalRender.color = Color.cyan;
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer != 11) return;
        else crTriggered = false;
        portalRender.color = Color.blue;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && crTriggered && !entering)
        {
            loadStage();
        }
    }
    public void loadStage()
    {
        if (stageData == null) { Debug.Log($"({gameObject.name}) StageData is null"); return; }
        entering = true;
        StartCoroutine(stageData.data.SwipeWhenTeleport ? SwipeLoad() : FadeLoad());
    }
    IEnumerator SwipeLoad()
    {
        int swipeDestination = (int)stageData.data.swipeDestination;
        RectTransform parentCanvas = new GameObject("swiperCanvas", typeof(RectTransform)).GetComponent<RectTransform>();
        DontDestroyOnLoad(parentCanvas.gameObject);

        parentCanvas.AddComponent<Canvas>();
        parentCanvas.AddComponent<CanvasScaler>();
        parentCanvas.AddComponent<GraphicRaycaster>();
        parentCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        parentCanvas.GetComponent<Canvas>().sortingOrder = 50;
        parentCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        parentCanvas.GetComponent<CanvasScaler>().referenceResolution = GameStatus.ScreenSize;

        var swiper = Instantiate(GameManager.Instance?.swipePrefab ?? localSwipePrefab, parentCanvas).GetComponent<RectTransform>();

        if (swipeDestination < 2)
        {
            float direction = swipeDestination * -2 + 1;
            swiper.anchoredPosition = new Vector3(swiper.anchoredPosition.x + tweenDistance * direction, swiper.anchoredPosition.y);
            yield return swiper.DOAnchorPosX(swiper.anchoredPosition.x - tweenDistance * direction, 0.65f).WaitForCompletion();
        }
        else
        {
            float direction = 5 - swipeDestination * 2;
            swiper.localScale = new Vector3(0.5625f, 1.7778f);
            swiper.localRotation = Quaternion.Euler(0, 0, 180 + 90 * direction);
            swiper.anchoredPosition = new Vector3(swiper.anchoredPosition.x, swiper.anchoredPosition.y - tweenDistance * 9 / 16 * direction);
            yield return swiper.DOAnchorPosY(swiper.anchoredPosition.y + tweenDistance * 9 / 16 * direction, 0.65f).WaitForCompletion();
        }

        if (GameManager.Instance == null)
        {
            Destroy(parentCanvas.gameObject);
            SceneManager.LoadScene(stageData.data.SceneName);
            yield break;
        }
        GameManager.Instance.CurrentStage = stageData;
        GameManager.Instance.GoStage(
            swiper,
            parentCanvas.gameObject,
            swipeDestination < 2,
            swipeDestination < 2 ?
            swipeDestination * 2 - 1 : 5 - swipeDestination * 2,
            tweenDistance * (swipeDestination < 2 ? 1 : 9f / 16f)
        );
    }
    IEnumerator FadeLoad()
    {
        yield return null;
    }
}
