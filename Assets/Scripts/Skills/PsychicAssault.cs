using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychicAssault : Skill
{
    void Awake()
    {
        SkillName = "Psychic Assault";
        BaseDamage = 3;
        TriggerName = "Psychic Assault";
        SkillNumber = 2;
    }
}
