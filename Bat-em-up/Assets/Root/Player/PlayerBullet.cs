using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBullet : MonoBehaviour, IStrikeable
{
    //public static Player_Bullet instance_PBullet;

    [SerializeField] private GetPlayerInstance pInstance;
    [SerializeField] private GetBulletInstance bInstance;

    // Variables for bullet state and behavior
    public bool inIdle = true;              // Indicates if the bullet is in idle state
    public bool wasHit = false;            // Indicates if the bullet was hit
    public bool startingSlowDown = false;  // Indicates if the bullet is starting to slow down
    public float timeBeforeSlowDown = 0.25f; // Time before the bullet starts to slow down
    public float smoothTime = 0.1f;         // Smooth time for velocity adjustments
    //private Player player;               // Reference to the player's transform

    // Variables for bullet properties
    public float power = 15f;                // Bullet power
    [Header("Speed")]
    [SerializeField] public float minSpeed = 5f; // Minimum bullet speed
    [SerializeField] public float maxSpeed = 30F; // Maximum bullet speed

    [Header("Scale")]
    public float minSize = 0.5f;           // Minimum bullet size
    public float maxSize = 7f;             // Maximum bullet size
    public float scaleUpFactor = 1.4f;     // Factor to scale up the bullet
    
    public float trailMinSize = 0.1f;
    public float trailMaxSize = 0.5f;


    [Header("Time")]
    public float maxSlowDuration = 15f;    // Maximum slow duration
    public float lerpTime;                 // Time for lerping
    private float currentLerpTime;         // Current lerping time

    // Variables for velocity and movement
    private float velocityTimeElapsed;     // Time elapsed for velocity adjustments
    [SerializeField] GameObject bulletGameObject; // Reference to the bullet's game object
    [SerializeField] private Rigidbody2D rigidBody; // Reference to the bullet's rigidbody
    public Vector2 currentVelocity = Vector2.zero; // Current velocity of the bullet


    [Header ("VFX")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private TrailRenderer prefabTrail;
    
    private TrailRenderer trail;


    private float initialScale;            // Initial scale of the bullet
    private Vector3 targetPosition;        // Target position for bullet movement
    private float currentSpeed;            // Current speed of the bullet



    private void Awake()
    {
        initialScale = transform.localScale.x;

        bInstance.bulletInstance = this;

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Boss>())
        {
            collision.GetComponent<Boss>().Hurt(bulletGameObject.transform.localScale.x);
            Vector2 collisionNormal =(transform.position - collision.transform.position).normalized;

            rigidBody.velocity = Vector2.Reflect(currentVelocity, collisionNormal);

            GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
        }
    }


    private void Start()
    {
        // Find the player's transform by name
        trail = GameObject.Instantiate(prefabTrail, transform.position, Quaternion.identity);

        rigidBody = GetComponent<Rigidbody2D>();
        initialScale = bulletGameObject.transform.localScale.x;
        rigidBody.velocity = new Vector2(-1, 0);
    }

    private void Update()
    {
        trail.transform.position = transform.position;
        if (wasHit)
        {
            // Handle behavior when the bullet was hit
            velocityTimeElapsed += Time.deltaTime;

            if (velocityTimeElapsed >= timeBeforeSlowDown)
            {
                currentVelocity = rigidBody.velocity;
                startingSlowDown = true;
                wasHit = false;
            }
        }

        if (startingSlowDown)
        {
            // Handle behavior when the bullet is starting to slow down
            if (transform.localScale.x > minSize)
            {
                currentLerpTime += Time.deltaTime / maxSlowDuration;
                lerpTime += Time.deltaTime;

                float t = Mathf.Clamp01(currentLerpTime);

                float newScale = Mathf.Lerp(maxSize, minSize, t);
                float newSpeed = Mathf.Lerp(maxSpeed, minSpeed, t);
                float newTrailSize = Mathf.Lerp(trailMaxSize, trailMinSize, t);

                transform.localScale = Vector3.one * newScale;
                rigidBody.velocity = rigidBody.velocity.normalized * newSpeed;
                trail.time = newTrailSize;
                currentVelocity = rigidBody.velocity;
            }
            else
            {
                inIdle = true;
                startingSlowDown = false;
                currentSpeed = currentVelocity.magnitude;
            }
        }

        if (inIdle)
        {
            
            // Handle behavior when the bullet is in idle state
            Vector3 directionToPlayer = (pInstance.playerInstance.transform.position - transform.position).normalized;
            targetPosition = pInstance.playerInstance.transform.position - directionToPlayer * 0.05f;
            Vector2 desiredVelocity = (targetPosition - transform.position).normalized * maxSpeed;
            Vector2 smoothedVelocity = Vector2.SmoothDamp(rigidBody.velocity, desiredVelocity, ref currentVelocity, smoothTime);
            rigidBody.velocity = smoothedVelocity;
            rigidBody.velocity = Vector2.Lerp(smoothedVelocity, Vector2.zero, 1.25f * Time.deltaTime);
        }
        currentVelocity = rigidBody.velocity;
    }

    public void Striking(Vector2 direction, float speed)
    {
        // var speed not use for this element
        
        inIdle = false;
        wasHit = true;
        startingSlowDown = false;
        velocityTimeElapsed = 0;
        float clampedScale = Mathf.Clamp(transform.localScale.x * scaleUpFactor, minSize, maxSize);
        transform.localScale = Vector3.one * clampedScale;
        currentLerpTime = Mathf.InverseLerp(maxSize, minSize, clampedScale);
        rigidBody.velocity = direction * Mathf.Lerp(maxSpeed, minSpeed, currentLerpTime);
        trail.time = Mathf.Lerp(trailMaxSize, trailMinSize, currentLerpTime);
        lerpTime = 0;
    }
}
