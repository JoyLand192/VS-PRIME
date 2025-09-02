using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private Color paused = new Color(0, 0, 0, 215f / 255f);
    #region Components
    CR cr;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] AudioSource[] musicSources;
    [SerializeField] DebugButtonManager debugButtonManager;
    [SerializeField] AudioManager audioManager;
    [SerializeField] Image blackScreen;
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] RectTransform settingsMenu;
    [SerializeField] RectTransform audioSettings;
    [SerializeField] RectTransform settingsTitle;
    #endregion
    public static List<Skill> Skills { get; private set; } = new List<Skill>();
    public GameObject swipePrefab;
    public GameObject hud;
    public GameObject skillSlotsParent;
    public GameObject skillSlot;
    public GameObject skillReadyFlash;
    public Dictionary<Skill, GameObject> equippedSkills = new();
    public RectTransform canvas50;
    public Image ultMeter;
    public Image ultVig;
    public Color crColor;
    public bool isSetting { get; private set; } = false;
    [SerializeField] CurrentStageData m_currentStage;
    public CurrentStageData CurrentStage
    {
        get => m_currentStage;
        set
        {
            Debug.Log($"{name}: Changing CurrentStageData");
            m_currentStage = value;
        }
    }
    public Vector2 CamMin
    {
        get
        {
            return CurrentStage?.CameraPosMin ?? new Vector2(-5000, -5000);
        }
        set
        {
            if (CurrentStage != null) CurrentStage.CameraPosMin = value;
            DefaultCamera.Instance.UpdateCameraCap();
        }
    }
    public Vector2 CamMax
    {
        get
        {
            return CurrentStage?.CameraPosMax ?? new Vector2(5000, 5000);
        }
        set
        {
            if (CurrentStage != null) CurrentStage.CameraPosMax = value;
            DefaultCamera.Instance.UpdateCameraCap();
        }
    }
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }


    void Awake()
    {
        cr = GameObject.Find("CR").GetComponent<CR>();
        pauseMenu.gameObject.SetActive(true);

        MusicVolumeChanged();
        audioManager.SFXVolumeChanged();

        instance = this;
        DontDestroyOnLoad(hud);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Skills = Resources.LoadAll<GameObject>("Skills").Select(prefab => prefab.GetComponent<Skill>()).ToList();
        Debug.Log($"[GameManager] Loaded skills: {Skills.Count}");

        foreach (Skill s in Skills)
        {
            Debug.Log($"{s.SkillName} ({s.SkillNumber}) has been Loaded.");
        }

        musicVolumeSlider.onValueChanged.AddListener(delegate {
            MusicVolumeChanged();
        });

        masterVolumeSlider.onValueChanged.AddListener(delegate {
            GameStatus.MasterVolume = masterVolumeSlider.value;
            MusicVolumeChanged();
            audioManager.SFXVolumeChanged();
        });
        if (CurrentStage == null) Debug.Log("GameManager => StageData Not Assigned");
    }
    public void EquipSkill(Skill skill)
    {
        if (equippedSkills.Count > 3 || equippedSkills.ContainsKey(skill))
        {
            Debug.Log($"[ {skill.SkillName} 스킬을 장착할 수 없습니다! ]");
            return;
        }
        var newSlot = Instantiate(skillSlot, skillSlotsParent.transform);
        newSlot.name = $"SkillSlot_{skill.SkillName}";
        equippedSkills.Add(skill, newSlot);
        newSlot.transform.Find("SkillSlot_Mask/icon").GetComponent<Image>().sprite = skill.Icon;
        newSlot.transform.Find("SkillSlot_Outline").GetComponent<Image>().color = crColor;
    }
    public void GoStage(RectTransform swiper, GameObject parent, bool horizontal, int side, float distance)
    {
        StartCoroutine(LoadStage(swiper, parent, horizontal, side, distance));
    }
    public IEnumerator LoadStage(RectTransform swiper, GameObject parent, bool horizontal, int side, float distance)
    {
        if (CurrentStage == null) { Debug.Log("SceneData is Null!!"); yield break; }

        AsyncOperation loader = SceneManager.LoadSceneAsync(CurrentStage.SceneName);
        loader.allowSceneActivation = true;
        yield return loader;
        yield return new WaitForEndOfFrame();

        cr = GameObject.Find("CR").GetComponent<CR>();
        cr.SceneInit();

        if (CurrentStage.ResetStats)
        {
            cr.UltimateAmount = 0;
        }

        var Teuler = swiper.localEulerAngles;
        swiper.anchoredPosition = Vector3.zero;
        swiper.localRotation = Quaternion.Euler(Teuler.x, Teuler.y, Teuler.z - 180f);

        if (horizontal)
        {   
            yield return swiper.DOAnchorPosX(distance * side, 0.7f).SetEase(Ease.OutCirc).WaitForCompletion();
        }
        else
        {
            yield return swiper.DOAnchorPosY(distance * side, 0.7f).SetEase(Ease.OutCirc).WaitForCompletion();
        }

        Destroy(parent);
    }
    public IEnumerator SetSkillCooldown(CR cr, Skill skill, float boost)
    {
        cr.cooldownSet.Add(skill);

        var fixedCooldown = skill.Cooldown * (1 - (boost / (boost + 100))); // 스킬가속 계산식
        var currentCooldown = fixedCooldown;
        var cooldownDisplay = equippedSkills[skill].transform.Find("SkillSlot_Cooldown").GetComponent<Image>();
        while (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            cooldownDisplay.fillAmount = currentCooldown / fixedCooldown;
            yield return null;
        }

        cr.cooldownSet.Remove(skill);

        var Tflash = Instantiate(skillReadyFlash, cooldownDisplay.transform.parent).transform;
        Tflash.SetSiblingIndex(Tflash.parent.childCount - 2);
        Tflash.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 1f).SetEase(Ease.OutCubic);
        Destroy(Tflash.gameObject, 1.2f);
    }
    public void MusicVolumeChanged()
    {
        foreach (var a in musicSources)
        {
            a.volume = musicVolumeSlider.value * GameStatus.MasterVolume;
        }
    }

    public void CallSfx(string sfx)
    {
        audioManager.PlaySfx((AudioManager.SfxList)Enum.Parse(typeof(AudioManager.SfxList), sfx), 100f);
    }
    public void CallSfx(string sfx, float volume)
    {
        audioManager.PlaySfx((AudioManager.SfxList)Enum.Parse(typeof(AudioManager.SfxList), sfx), volume < 100f ? volume : 100f);
    }

    private void Update()
    {
        if (cr != null && cr.transform.position.y < CamMin.y - 5)
        {
            cr.transform.position = new Vector3(0, Mathf.Max(0, CamMin.y) + 3);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameStatus.IsPause) Resume();
            else Pause();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            debugButtonManager.SummonOrangeMushroom();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            debugButtonManager.SummonSpikyMushroom();
        }
    }

    public void Pause()
    {
        CallSfx("Select");
        GameStatus.IsPause = true;
        Time.timeScale = 0;
        blackScreen.color = paused;
        pauseMenu.Pause();
    }
    public void Resume()
    {
        if (isSetting)
        {
            CallSfx("Remote");
            audioSettings.DOKill(true);
            settingsTitle.DOKill(true);
            isSetting = false;
            pauseMenu.Pause();
            settingsMenu.gameObject.SetActive(false);

            return;
        }

        CallSfx("Remote");
        GameStatus.IsPause = false;
        Time.timeScale = 1;
        blackScreen.color = Color.clear;
        pauseMenu.Resume();
        pauseMenu.ty.SetActive(false);
    }
    public void Settings()
    {
        pauseMenu.Resume();
        isSetting = true;
        CallSfx("Select");
        Debug.Log("Have a good time!");

        pauseMenu.Menu.gameObject.SetActive(false);
        audioSettings.anchoredPosition = new Vector3(audioSettings.anchoredPosition.x, audioSettings.anchoredPosition.y - 110f);
        audioSettings.DOAnchorPosY(audioSettings.anchoredPosition.y + 110f, 0.5f).SetEase(Ease.OutCirc).SetUpdate(true);

        settingsMenu.gameObject.SetActive(true);
        settingsTitle.anchoredPosition = new Vector3(settingsTitle.anchoredPosition.x, settingsTitle.anchoredPosition.y + 100f);
        settingsTitle.DOAnchorPosY(settingsTitle.anchoredPosition.y - 100f, 0.5f).SetEase(Ease.OutCirc).SetUpdate(true);
    }
    public void Exit()
    {
        Debug.Log("Bye bye!");
        Application.Quit();
    }
}
