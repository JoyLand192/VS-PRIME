using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class TheNew : CR
{
    GameObject curISEffect;
    GameObject curMeleeEffect;
    GameObject curUltEffect;
    [SerializeField] private int meleeProgress = 1;
    public int MeleeProgress
    {
        get => meleeProgress;
        set
        {
            anim.SetInteger("MeleeProgress", value);
            meleeProgress = value;
        }
    }
    public float meleeTimer = 0;
    public float meleeTimeLimit;
    enum SkillSet { DoubleSlash, PsychicAssault, Ultimate };
    [SerializeField] KeyCode meleeKey = KeyCode.A;
    [SerializeField] KeyCode ultKey = KeyCode.Q;
    [SerializeField] GameObject particle_prefab_ds;
    [SerializeField] GameObject particle_prefab_ult;
    [SerializeField] GameObject illusionSwordSlash;
    [SerializeField] GameObject[] meleeEffectPrefabs;
    [SerializeField] GameObject ultimateLaser;

    protected override void Init()
    {
        base.Init();

        speedCR = 13.3f;
        damageBase = 50f;
        jumpPower = 17f;
        maxGravity = 30f;
        abilityHaste = 0f;
        meleeTimeLimit = 0.72f;
        GameManager.Instance.EquipSkill(GetSkill(1));
        GameManager.Instance.EquipSkill(GetSkill(2));

        skillEquippedList.Add(KeyCode.D, GetSkill(1));
        skillEquippedList.Add(KeyCode.S, GetSkill(2));
        skillKeys.Add(KeyCode.D);
        skillKeys.Add(KeyCode.S);
    }

    protected override void Update()
    {
        base.Update();

        meleeTimer += Time.deltaTime;
        if (meleeTimer > meleeTimeLimit) { MeleeProgress = 1; meleeTimer = 0; }

        if (Input.GetKeyDown(meleeKey))
        {
            MeleeAttack();
        }

        if (Input.GetKeyDown(ultKey))
        {
            UltimateSkillCast();
        }
    }
    void UltimateSkillCast()
    {
        if (canMove == false || !ultCharged) return;

        anim.ResetTrigger("WALK");
        stateCR = State.Channeling;
        canMove = false;
        rb.velocity = new Vector2(0, rb.velocity.y);

        UltimateAmount = 0f;
        ultCharged = false;
        ultimateVig.gameObject.SetActive(false);
        anim.SetTrigger("ULTIMATE");
        Destroy(ultChargeEff);
    }
    public void UltimateEffGen()
    {
        curUltEffect = Instantiate(ultimateLaser, transform);
    }
    public void UltHitTest(int t)
    {
        Vector2 curPos = transform.position;
        Collider2D[] colliders = null;
        switch (t)
        {
            case 1:
                {
                    colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(-2f * direction, -1), curPos + new Vector2(1.6f * direction, 1), LayerMask.GetMask("E-Units"));
                    break;
                }
            case 2:
                {
                    colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(-0.5f * direction, -1), curPos + new Vector2(30f * direction, 1), LayerMask.GetMask("E-Units"));
                    break;
                }
        }

        if (colliders != null)
        {
            foreach (var hit in colliders)
            {
                if (hit.TryGetComponent<EUnits>(out var eu)) eu.GetDamaged(damageCR * GetSkill(3).BaseDamage * (-2 * t + 5));
                //GenSkillEffect(t + 2, hit.transform.position);

                var pps = Instantiate(particle_prefab_ds, hit.transform.localPosition, Quaternion.identity);
                if (pps.TryGetComponent<ParticleSystem>(out var p)) p.Play();
                Destroy(pps.gameObject, 2f);
            }
        }

        if (colliders.Length > 0)
        {
            CallSfx("Damage");
        }
    }
    public void DoubleSlash_HitTest()
    {
        float DSDamage = GetSkill(1).BaseDamage * damageCR;
        Vector2 curPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(2.8f * direction, -1), curPos + new Vector2(-2f * direction, 1), LayerMask.GetMask("E-Units"));
        Vector2 DS_CenterPos = new Vector2();

        foreach (Collider2D E_UnitsHit in colliders)
        {
            if (E_UnitsHit.transform == null) continue;
            DS_CenterPos += (Vector2)E_UnitsHit.transform.position;
            if (E_UnitsHit.TryGetComponent<EUnits>(out var eu)) eu.GetDamaged(DSDamage);
            GenSkillEffect((int)SkillSet.DoubleSlash, E_UnitsHit.transform.localPosition);

            UltimateAmount += 3.5f;
            var pps = Instantiate(particle_prefab_ult, E_UnitsHit.transform.localPosition, Quaternion.identity);
            if (pps.TryGetComponent<ParticleSystem>(out var p)) p.Play();
            Destroy(pps.gameObject, 2f);
        }

        if (colliders.Length > 0)
        {
            DS_CenterPos /= colliders.Length;
            CallSfx("Damage");
        }
    }
    public void UltCamera(int t)
    {
        switch (t)
        {
            case 1:
                {
                    StartCoroutine(DefaultCamera.Instance.Shake(8f, 8f, 0.4f));
                    StartCoroutine(DefaultCamera.Instance.Scale(8f, 0.4f, Ease.OutCirc));
                    break;
                }
            case 2:
                {
                    StartCoroutine(DefaultCamera.Instance.MoveTween(transform.position + new Vector3(direction * 5, 2), 1f, Ease.InOutCirc));
                    StartCoroutine(DefaultCamera.Instance.Scale(12f, 1f, Ease.InOutCirc));
                    break;
                }
            case 3:
                {
                    StartCoroutine(DefaultCamera.Instance.Shake(1f, 1f, 1f, false));
                    break;
                }
            case 4:
                {
                    DefaultCamera.Instance.followingTarget = true;
                    StartCoroutine(DefaultCamera.Instance.Scale(10f, 1.2f, Ease.OutCirc));
                    break;
                }
        }
    }
    void MeleeAttack()
    {
        if (canMove == false) return;

        anim.ResetTrigger("WALK");
        stateCR = State.Channeling;
        canMove = false;
        rb.velocity = new Vector2(0, rb.velocity.y);

        anim.SetTrigger("MeleeTrigger");
        meleeTimer = 0;

        Collider2D[] colliders = null;
        Vector2 curPos = transform.position;
        switch (MeleeProgress)
        {
            case 1:
                {
                    colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(-0.58f * direction, -1), curPos + new Vector2(2.6f * direction, 1), LayerMask.GetMask("E-Units"));
                    break;
                }
            case 2:
                {
                    colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(-0.5f, -1), curPos + new Vector2(3.2f * direction, 1), LayerMask.GetMask("E-Units"));
                    break;
                }
            case 3:
                {
                    StartCoroutine(Melee3Dash());
                    break;
                }
            case 4:
                {
                    colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(-0.4f * direction, -1), curPos + new Vector2(2f * direction, 1.77f), LayerMask.GetMask("E-Units"));
                    break;
                }
        }

        if (MeleeProgress != 3 && colliders != null)
        {
            foreach (var hit in colliders)
            {
                if (hit.TryGetComponent<EUnits>(out var eu)) eu.GetDamaged(MeleeProgress < 3 ? damageCR : damageCR * 1.7f);
                GenSkillEffect(2, hit.transform.position);
                UltimateAmount += 0.5f;
            }
            if (colliders.Length > 0)
            {
                int t = UnityEngine.Random.Range(1, 5);
                string s;
                switch (t)
                {
                    case 1:
                        {
                            s = "Punch1";
                            break;
                        }
                    case 2:
                        {
                            s = "Punch2";
                            break;
                        }
                    case 3:
                        {
                            s = "Kick1";
                            break;
                        }
                    default:
                        {
                            s = "Kick2";
                            break;
                        }
                }
                CallSfx(s);
            }
        }
    }
    public IEnumerator Melee3Dash()
    {
        dashing = true;
        float dashDistance = 5f;
        var startPosition = transform.position;
        var destination = transform.position + new Vector3(direction * dashDistance, 0);

        float t = 0;
        float duration = 0.3f;
        float attackCycle = 0.15f;

        float d = attackCycle;

        Dictionary<Transform, Vector3> hittenUnits = new();

        while (t < duration)
        {
            float f = Mathf.Clamp01(t / duration);
            float outCircEase = Mathf.Sqrt(1f - Mathf.Pow(f - 1f, 2f));

            RaycastHit2D platformsOnForward = Physics2D.Raycast(transform.position + new Vector3(direction * dashDistance / 5 + 0.3f, -0.8f), Vector2.up, 1.6f, LayerMask.GetMask("Platforms"));
            Debug.DrawRay(transform.position + new Vector3(direction * dashDistance / 5 + 0.3f, -0.8f), Vector2.up * 1.6f, Color.cyan);
            Debug.DrawRay(transform.position, Vector2.right * direction * dashDistance / 5, Color.cyan);

            if (platformsOnForward.collider == null) transform.position = Vector3.Lerp(startPosition, destination, outCircEase);
            if (hittenUnits != null)
            {
                foreach (var h in hittenUnits)
                {
                    if (h.Key == null) { hittenUnits.Remove(h.Key); continue; }
                    h.Key.position = transform.position + h.Value;
                }
            }

            t += Time.deltaTime;
            d += Time.deltaTime;

            if (d >= attackCycle)
            {
                d -= attackCycle;
                var colliders = Physics2D.OverlapAreaAll((Vector2)transform.position + new Vector2(-1.6f * direction, -1), (Vector2)transform.position + new Vector2(1.15f * direction, 1), LayerMask.GetMask("E-Units"));
                if (colliders != null)
                {
                    foreach (var c in colliders)
                    {
                        Vector3 gap = new Vector3(Mathf.Clamp(transform.position.x - c.transform.position.x, -0.8f, 1f), c.transform.position.y);
                        if (c.TryGetComponent<EUnits>(out var eu)) eu.GetDamaged(damageCR * 0.5f);
                        if (!hittenUnits.ContainsKey(c.transform)) hittenUnits.Add(c.transform, gap);
                        UltimateAmount += 0.25f;
                    }
                    if (colliders.Length > 0) CallSfx("Punch1");
                }
            }

            yield return null;
        }

        anim.SetTrigger("Melee3End");
        if (curMeleeEffect == null) yield break;
        curMeleeEffect.GetComponent<Animator>().SetTrigger("isEnd");
        dashing = false;

        yield return null;
    }
    public void MeleeEffGen()
    {
        curMeleeEffect = Instantiate(meleeEffectPrefabs[MeleeProgress - 1], transform);
    }

    public void IllusionSwordEffGen()
    {
        curISEffect = Instantiate(illusionSwordSlash, transform);
    }

    public void UpdateMeleeProgress()
    {
        MeleeProgress = MeleeProgress % 4 + 1;
    }


    public void PsychicAssault_HitTest()
    {
        float DSDamage = GetSkill(2).BaseDamage * damageCR;
        float dis = 2.5f * direction;
        Vector2 curPos = transform.position;

        Collider2D[] colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(dis * 2.4f, -1), curPos + new Vector2(0, 2.1f), LayerMask.GetMask("E-Units"));

        foreach (Collider2D E_UnitsHit in colliders)
        {
            E_UnitsHit.transform.GetComponent<EUnits>().GetDamaged(DSDamage);
            GenSkillEffect((int)SkillSet.PsychicAssault, E_UnitsHit.transform.localPosition);
            UltimateAmount += 2f;
        }

        if (colliders.Length > 0)
        {
            CallSfx("Damage");
        }

        Instantiate(effectManager.LoadEtcEffect(1), transform.localPosition, Quaternion.identity);
        Instantiate(effectManager.LoadEtcEffect(3), transform.localPosition + new Vector3(dis, 0), Quaternion.identity);

        StartCoroutine(PsychicAssault_Disappear());

    }

    IEnumerator PsychicAssault_Disappear()
    {
        float tmp = anim.speed;
        float patime = 0f;
        float tmp2 = direction;

        anim.speed = 0f;
        box.enabled = false;
        render.enabled = false;
        rb.isKinematic = true;

        CallSfx("Taat", 40f);
        Destroy(curISEffect);

        while (patime < 0.16f)
        {
            float T_dashingDistance = tmp2 * 44f * Time.deltaTime;

            RaycastHit2D platformsOnForward = Physics2D.Raycast(transform.position + new Vector3(T_dashingDistance + 0.3f, -0.8f), Vector2.up, 1.6f, LayerMask.GetMask("Platforms"));
            Debug.DrawRay(transform.position + new Vector3(T_dashingDistance + 0.3f, -0.8f), Vector2.up * 1.6f, Color.cyan);
            Debug.DrawRay(transform.position, Vector2.right * T_dashingDistance, Color.blue);

            if (platformsOnForward.collider == null) transform.localPosition += new Vector3(T_dashingDistance, 0);

            patime += Time.deltaTime;
            yield return null;
        }

        rb.isKinematic = false;
        box.enabled = true;
        anim.speed = tmp;
        render.enabled = true;

        CallSfx("TP", 40f);
    }

    void GenSkillEffect(int SkillNum, Vector3 Pos)
    {
        Vector3 RandomPos = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f), 0);
        Instantiate(effectManager.LoadSkillEffect(SkillNum), Pos + RandomPos, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)));
    }
}
