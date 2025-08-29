using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStageData", menuName = "StageChanger/StageData")]
public class CurrentStageData : ScriptableObject
{
    public string SceneName;
    public bool ResetStats;
    public bool SwipeWhenTeleport;
    public Vector2 CameraPosMin;
    public Vector2 CameraPosMax;
    public enum SwipeDestination { Left, Right, Up, Down }
    public SwipeDestination swipeDestination;
}
