using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyMovementData", menuName = "Enemy/Enemy Movement Data")]
public class EnemyMovementData : ScriptableObject
{
    public float moveSpeed = 2f;
    [SerializeField] public Transform[] waypoints;
}

