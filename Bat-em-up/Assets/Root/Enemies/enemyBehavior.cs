using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewEnemyBehavior", menuName = "Enemy/Enemy Behavior")]
public class EnemyBehavior : ScriptableObject
{
    public enum Behavior
    {
        statique,
        point_de_fuite,
        waypoint,
        spline_waypoint,
    }

    public Behavior behavior = Behavior.statique;
    public float moveSpeed = 5f;
    public int health = 100;
    public float fireRate = 2f;
}