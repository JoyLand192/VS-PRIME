using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    Object[] MobPrefabs;
    public GameObject[] HealthBarPrefabs;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        MobPrefabs = Resources.LoadAll("Mobs")
            .OrderBy(p => p.name)
            .ToArray();

        HealthBarPrefabs = Resources.LoadAll<GameObject>("UI/HealthBar")
            .OrderBy(p => p.name)
            .ToArray();
    }

    public Object LoadMob(int MobNumber)
    {
        return MobPrefabs[MobNumber - 1];
    }

    public GameObject LoadHealthBar(int num)
    {
        return HealthBarPrefabs[num - 1];
    }
}
