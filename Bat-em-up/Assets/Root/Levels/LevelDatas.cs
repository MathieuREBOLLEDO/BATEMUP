using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/Data")]

public class LevelDatas : ScriptableObject
{
    public ScrollingEffect scrolType;
    public float time;
    public float score;
}
