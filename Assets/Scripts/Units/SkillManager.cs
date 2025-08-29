using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<Skill> SL_Temp = new List<Skill>();
    public Dictionary<int, Skill> SkillList = new Dictionary<int, Skill>();

    void Awake()
    {
        Init();
    }

    void Init()
    {
        foreach (Skill x in SL_Temp)
        {
            SkillList.Add(x.SkillNumber, x);
        }
    }
    public Skill LoadSkill(int SkillNumber)
    {
        return SkillList[SkillNumber];
    }
}