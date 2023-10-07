using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public EnemyBehavior enemyBehavior;
    public EnemyWeapon enemyWeapon;
    //public EnemyMovementData movementData;
    public Transform[] waypoints;
    private Transform weaponSpawnPoint;

    private int currentWaypointIndex = 0;

    public GameObject bullet;
    [SerializeField] private Transform muzzle;

    public SplineContainer spline;

    public Transform[] splinePoints; // The control points of your spline
    public float movementSpeed = 2.0f; // Speed of movement
    private float distanceTraveled = 0.0f;

    private int currentSegment = 0;
    private float t = 0.0f;
    private bool canShoot;

    Player player;

    private float nextFireTime;

    private void Start()
    {
        weaponSpawnPoint = muzzle;

        // Access the Player Singleton instance.
        player = Player.instance;

        if (enemyBehavior.behavior == 0)
        {
            canShoot = true;
        }
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[currentWaypointIndex].position;
        }

        // Set initial enemy behavior values
        // Set initial enemy weapon values
    }

    private void Update()
    {
        if (enemyBehavior.behavior != 0)
        {
            Move();
        }
        enemyWeapon.Fire();


        Fire();
    }

    private void Move()
    {
        if (waypoints.Length > 0 && currentWaypointIndex < waypoints.Length)
        {
            Vector3 direction = waypoints[currentWaypointIndex].position - transform.position;
            transform.Translate(direction.normalized * enemyBehavior.moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
            {
                currentWaypointIndex++;


                
                if (currentWaypointIndex >= waypoints.Length)
                {
                    canShoot = true;
                    //currentWaypointIndex = 0;
                    //var dir = player.transform.position - transform.position;
                    //var angle = Mathf.Atan2(dir.y, dir.x)* Mathf.Rad2Deg - 180;
                    //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    //transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector2.up*Quaternion.AngleAxis());
                }
                

            }
        }


    }

    private void Fire()
    {
        if (canShoot)
        {
            var dir = player.transform.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (Time.time >= nextFireTime)
            {
                for (int i = 0; i < 3; i++)
                {
                    var dir2 = player.transform.position - transform.position;
                    var angle2 = Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg - 180 -25 + 25*i;
                    transform.rotation = Quaternion.AngleAxis(angle2, Vector3.forward);

                    GameObject tmpBullet = GameObject.Instantiate(bullet, weaponSpawnPoint.position, transform.rotation);
                    //tmpBullet.transform.rotation = 
                    tmpBullet.GetComponent<Bullet_Behavior>().speed = 5f;

                }
                //weaponSpawnPoint
               // GameObject tmpBullet = GameObject.Instantiate(bullet, weaponSpawnPoint);
                // Implement enemy firing using enemyWeapon properties
                nextFireTime = Time.time + enemyWeapon.fireInterval;
            }
        }
    }
}