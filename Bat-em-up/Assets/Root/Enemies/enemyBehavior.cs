using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemyBehavior", menuName = "Enemy/Enemy Behavior")]
public class EnemyBehavior : ScriptableObject
{
    public float moveSpeed = 5f;
    public int health = 100;
    public float fireRate = 2f;
}