using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class TheNew : CR
{
    GameObject curMeleeEffect;
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
    enum SkillSet { DoubleSlash, PsychicAssault };
    [SerializeField] KeyCode meleeKey = KeyCode.A;
    [SerializeField] GameObject particle_prefab_ds;
    [SerializeField] GameObject[] meleeEffectPrefabs;

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
                        colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(-0.28f * direction, -1), curPos + new Vector2(2.6f * direction, 1), LayerMask.GetMask("E-Units"));
                        break;
                    }
                case 2:
                    {
                        colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(0, -1), curPos + new Vector2(3.2f * direction, 1), LayerMask.GetMask("E-Units"));
                        break;
                    }
                case 3:
                    {
                        StartCoroutine(Melee3Dash());
                        break;
                    }
                case 4:
                    {
                        colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(0.2f * direction, -1), curPos + new Vector2(2f * direction, 1.77f), LayerMask.GetMask("E-Units"));
                        break;
                    }
            }

            if (MeleeProgress != 3 && colliders != null)
            {
                foreach (var hit in colliders)
                {
                    if (hit.TryGetComponent<EUnits>(out var eu)) eu.GetDamaged(MeleeProgress < 3 ? damageCR : damageCR * 1.7f);
                    GenSkillEffect(2, hit.transform.position);
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

            Debug.Log($"Melee {MeleeProgress}");
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
        while (t < duration)
        {
            float f = Mathf.Clamp01(t / duration);
            float outCircEase = Mathf.Sqrt(1f - Mathf.Pow(f - 1f, 2f));

            RaycastHit2D platformsOnForward = Physics2D.Raycast(transform.position + new Vector3(outCircEase * dashDistance + 0.3f, -0.8f), Vector2.up, 1.6f, LayerMask.GetMask("Platforms"));
            Debug.DrawRay(transform.position + new Vector3(outCircEase * dashDistance + 0.3f, -0.8f), Vector2.up * 1.6f, Color.cyan);
            Debug.DrawRay(transform.position, Vector2.right * outCircEase * dashDistance, Color.blue);

            if (platformsOnForward.collider == null) transform.position = Vector3.Lerp(startPosition, destination, outCircEase);

            t += Time.deltaTime;
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

    public void UpdateMeleeProgress()
    {
        MeleeProgress = MeleeProgress % 4 + 1;
    }

    public void DoubleSlash_HitTest()
    {
        float DSDamage = GetSkill(1).BaseDamage * damageCR;
        Vector2 curPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapAreaAll(curPos + new Vector2(1.3f * direction, -1), curPos + new Vector2(-0.7f * direction, 1), LayerMask.GetMask("E-Units"));
        Vector2 DS_CenterPos = new Vector2();

        foreach (Collider2D E_UnitsHit in colliders)
        {
            DS_CenterPos += (Vector2)E_UnitsHit.transform.position;
            if (E_UnitsHit.TryGetComponent<EUnits>(out var eu)) eu.GetDamaged(DSDamage);
            GenSkillEffect((int)SkillSet.DoubleSlash, E_UnitsHit.transform.localPosition);

            UltimateAmount += 3.5f;
            ParticleSystem pps = Instantiate(particle_prefab_ds, E_UnitsHit.transform.localPosition, Quaternion.identity).GetComponent<ParticleSystem>();
            pps.Play();
            Destroy(pps.gameObject, 2f);
        }

        if (colliders.Length > 0)
        {
            DS_CenterPos /= colliders.Length;
            CallSfx("Damage");
        }
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
