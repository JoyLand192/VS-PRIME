using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    GameObject[] SkillEffects, EtcEffects, DamageText;
    void Awake()
    {
        SkillEffects = Resources.LoadAll<GameObject>("SkillEffects")
            .OrderBy(p => p.name)
            .ToArray();
        EtcEffects = Resources.LoadAll<GameObject>("EtcEffects")
            .OrderBy(p => p.name)
            .ToArray();
        DamageText = Resources.LoadAll<GameObject>("UI/DamageText")
            .OrderBy(p => p.name)
            .ToArray();
    }
    public GameObject LoadSkillEffect(int SkillNumber)
    {
        return SkillEffects[SkillNumber];
    }

    public GameObject LoadEtcEffect(int EffNumber)
    {
        return EtcEffects[EffNumber - 1];
    }

    public GameObject LoadDamageText(int Number)
    {
        return DamageText[Number - 1];
    }
}
