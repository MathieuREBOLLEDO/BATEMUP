using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyWeapon", menuName = "Enemy/Enemy Weapon")]
public class EnemyWeapon : ScriptableObject
{
    public GameObject projectilePrefab;
    public float fireInterval = 1f;
    public float projectileSpeed = 10f;
    public int damage = 10;

    public void Fire ()
    {

    }

}
