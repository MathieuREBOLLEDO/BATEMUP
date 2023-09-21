using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyBehavior enemyBehavior;
    public EnemyWeapon enemyWeapon;
    //public EnemyMovementData movementData;
    public Transform [] waypoints;
    private Transform weaponSpawnPoint;

    private int currentWaypointIndex = 0;

    private float nextFireTime;

    private void Start()
    {
        weaponSpawnPoint = transform.Find("WeaponSpawnPoint");


        if (waypoints.Length > 0)
        {
            transform.position = waypoints[currentWaypointIndex].position;
        }

        // Set initial enemy behavior values
        // Set initial enemy weapon values
    }

    private void Update()
    {
        Move();
        Fire();
    }

    private void Move()
    {
        if ( waypoints.Length > 0)
        {
            Vector3 direction = waypoints[currentWaypointIndex].position - transform.position;
            transform.Translate(direction.normalized * enemyBehavior.moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = 0;
                }
            }
        }
    }

    private void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            // Implement enemy firing using enemyWeapon properties
            nextFireTime = Time.time + enemyWeapon.fireInterval;
        }
    }
}