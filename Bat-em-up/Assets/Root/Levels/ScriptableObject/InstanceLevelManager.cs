using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="newInstanceLevel", menuName ="Level/Instance")]

public class InstanceLevelManager : ScriptableObject
{
    public LevelManager levelInstance;
}
