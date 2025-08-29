using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/SkillData")]
public class Skill : ScriptableObject
{
    public string SkillName;
    public string TriggerName;
    public int SkillNumber;
    public float BaseDamage;
    public float Cooldown;
    public bool Cancelable;
    public Sprite Icon;
}
