using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButtonManager : MonoBehaviour
{
    [SerializeField] CR cr;
    [SerializeField] UnitManager unitManager;
    [SerializeField] DefaultCamera CAM;
    float X, Y;
    int test = 1;
    void Awake()
    {
        X = CAM.XOffset;
        Y = CAM.YOffset;
    }
    public void XInc()
    {
        X++;
        CAM.SetOffset(x: X);
    }
    public void YInc()
    {
        Y++;
        CAM.SetOffset(y: Y);
    }
    public void XDec()
    {
        X--;
        CAM.SetOffset(x: X);
    }
    public void YDec()
    {
        Y--;
        CAM.SetOffset(y: Y);
    }
    public void SummonOrangeMushroom()
    {
        test++;
        Object Mushroom = Instantiate(unitManager.LoadMob(1), new Vector3(0, 3, 0), Quaternion.identity);
        Mushroom.name = $"Orange Mushroom {test}";
    }
    public void SummonSpikyMushroom()
    {
        test++;
        Object Mushroom = Instantiate(unitManager.LoadMob(2), new Vector3(0, 3, 0), Quaternion.identity);
        Mushroom.name = $"Orange Mushroom {test}";
    }
    public void HalfUlti()
    {
        cr.UltimateAmount += 50f;
    }
}
