using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CR : MonoBehaviour
{
    #region instance
    protected Rigidbody2D rb;
    protected Animator anim;
    protected BoxCollider2D box;
    protected Renderer render;
    protected RaycastHit2D hitGround;
    protected SkillManager skillManager;
    protected AudioManager audioManager;
    protected EffectManager effectManager;
    protected State stateCR;
    #endregion
    protected enum State { Idle, Moving, Channeling, Busy };
    protected Dictionary<KeyCode, Skill> skillEquippedList = new Dictionary<KeyCode, Skill>();
    protected List<KeyCode> skillKeys = new();
    protected Vector3 currentVelocity;
    protected bool dashing = false;
    protected Image ultimateCharge
    {
        get
        {
            return GameManager.Instance.ultMeter;
        }
    }
    protected Image ultimateVig
    {
        get
        {
            return GameManager.Instance.ultVig;
        }
    }

    #region Stats
    public float speedCR;
    public float currentDirection;
    public float jumpPower;
    public float abilityHaste;
    public float maxGravity;
    public bool jumping, onAir, canMove, ultCharged;
    public float damageBase = 10f;
    public float damageCR;
    public float direction;
    public HashSet<Skill> cooldownSet = new();

    protected float m_ultimateAmount;
    public float UltimateAmount
    {
        get => m_ultimateAmount;
        set
        {
            if (value >= 100f)
            {
                m_ultimateAmount = 100f;
                if (!ultCharged) UltChargedEffGen();
            }
            if (value <= 0) m_ultimateAmount = 0;
            else m_ultimateAmount = value;
        }
    }
    #endregion

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        render = GetComponent<Renderer>();
        skillManager = GameObject.Find("GameManager").GetComponentInChildren<SkillManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        stateCR = State.Idle;
        canMove = true;
        direction = transform.localScale.x >= 0 ? -1 : 1;
    }
    protected void Moving()
    {
        float T_dir;
        if (!canMove) return;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            T_dir = 1;
            rb.transform.localScale = new Vector2(currentDirection * rb.transform.localScale.y * -1, rb.transform.localScale.y);
            stateCR = State.Moving;
            anim.SetTrigger("WALK");
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            T_dir = -1;
            rb.transform.localScale = new Vector2(currentDirection * rb.transform.localScale.y * -1, rb.transform.localScale.y);
            stateCR = State.Moving;
            anim.SetTrigger("WALK");
        }
        else
        {
            anim.ResetTrigger("WALK");
            T_dir = 0;
        }

        if (currentDirection != T_dir)
        {
            transform.position += new Vector3(T_dir * 0.4f, 0);
            currentDirection = T_dir;
        }
    }
    public void SceneInit()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        render = GetComponent<Renderer>();
        skillManager = GameObject.Find("GameManager").GetComponentInChildren<SkillManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        stateCR = State.Idle;
        canMove = true;
        direction = transform.localScale.x >= 0 ? -1 : 1;
    }
    protected void TestSkillKeys()
    {
        foreach (KeyCode key in skillKeys)
        {
            if (Input.GetKeyDown(key))
            {
                if (skillEquippedList.TryGetValue(key, out Skill value) && value != null)
                {
                    if (dashing || !canMove && !value.Cancelable || cooldownSet.Contains(value)) { Debug.Log($"{{ {value.SkillName} 스킬을 아직 사용할 수 없습니다. }}"); return; }
                    ChannelSkillCast(value);
                    canMove = false;
                }
                else Debug.Log($"{key}키 스킬 : 장착되지 않음");
            }
        }
    }

    protected void ChannelSkillCast(Skill skill)
    {
        anim.ResetTrigger("WALK");
        anim.SetTrigger(skill.TriggerName);
        stateCR = State.Channeling;
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (skill.Cooldown > 0.05f) StartCoroutine(GameManager.Instance.SetSkillCooldown(this, skill, abilityHaste));
    }

    protected Skill GetSkill(int Number)
    {
        return skillManager.LoadSkill(Number);
    }

    protected void GetDirection()
    {
        if (Input.GetKey(KeyCode.RightArrow)) direction = 1;
        else if (Input.GetKey(KeyCode.LeftArrow)) direction = -1;
    }
    protected void StopChannel()
    {
        Debug.Log("Channel End");
        stateCR = State.Idle;
        canMove = true;
    }

    protected virtual void Update()
    {
        damageCR = UnityEngine.Random.Range(0.8f, 1.2f) * damageBase;
        if (GameStatus.IsPause) return;
        // 채널링 시 이동불가 관리
        switch (stateCR)
        {
            case State.Channeling:
            case State.Busy:
                canMove = false;
                break;
        }

        currentVelocity = rb.velocity;
        hitGround = Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y * 1.15f, LayerMask.GetMask("Platforms"));
        Debug.DrawRay(transform.position, Vector2.down * transform.localScale.y * 1.15f, Color.red);

        if (Input.anyKeyDown) TestSkillKeys();
        if (Input.GetKeyDown(KeyCode.Space) && !onAir) jumping = true;


        if (currentVelocity.y < maxGravity * -1)
        {
            currentVelocity.y = maxGravity * -1;
            rb.velocity = currentVelocity;
        }

        Moving();
        UpdateUltimateMeter();
        GetDirection();
    }

    protected virtual void FixedUpdate()
    {
        if (hitGround.collider == null)
        {
            onAir = true;
            if (!jumping) anim.SetTrigger("FALLING");
        }
        else
        {
            onAir = false;
            anim.ResetTrigger("FLOATING");
            anim.ResetTrigger("FALLING");
        }
        if (canMove)
        {
            rb.velocity = new Vector2(speedCR * currentDirection, rb.velocity.y);
            if (jumping)
            {
                rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                jumping = false;
                anim.SetTrigger("FLOATING");
            }
        }
    }

    protected void UltChargedEffGen()
    {
        var UltChargeEff = Instantiate(effectManager.LoadEtcEffect(4), transform);
        if (!ultCharged)
        {
            ultCharged = true;
            CallSfx("PowerCharge");
            ultimateVig.DOColor(new Color(1, 1, 1, 160f / 255f), 1f);
        }
    }

    public void CallSfx(string sfx)
    {
        audioManager.PlaySfx((AudioManager.SfxList)Enum.Parse(typeof(AudioManager.SfxList), sfx), 100f);
    }

    public void CallSfx(string sfx, float volume)
    {
        audioManager.PlaySfx((AudioManager.SfxList)Enum.Parse(typeof(AudioManager.SfxList), sfx), volume < 100 ? volume : 100);
    }
    
    public void UpdateUltimateMeter()
    {
        float lerpf = 4f;
        float tmp = ultimateCharge.fillAmount;
        ultimateCharge.fillAmount = Mathf.Lerp(tmp, UltimateAmount / 100f, lerpf * Time.deltaTime);
    }
}
