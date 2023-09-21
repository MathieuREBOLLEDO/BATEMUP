using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Bullet : MonoBehaviour
{
    public bool inIdle = true;
    public bool wasHit = false;
    public bool startingSlowDown = false;
    public float timeBeforeSlowDown = 0.25f;
    public float smoothTime = 0.5f;
    private Transform player;


    public float power = 15f;
    [Header("Speed")]

    [SerializeField] public float minSpeed = 5f;
    [SerializeField] public float maxSpeed = 30F;

    [Header("Scale")]

    public float minSize = 0.5f;
    public float maxSize = 7f;
    public float scaleUpFactor = 1.4f;
    //private Vector3 currentMaxScale;

    [Header("Time")]
    public float maxSlowDuration = 15f;
    public float lerpTime;
    private float currentLerpTime;

    //public float slowDownFactor = 0.1f;
    //public float scaleDownFactor = 0.1f;


    private float velocityTimeElapsed;
    [SerializeField] GameObject bulletGameObject;

    [SerializeField] private Rigidbody2D rigidBody;
    public Vector2 currentVelocity = Vector2.zero;

    private float initialScale;
    //private float targetScale = 0;
    //private float originalVelocityMagnitude;

    private Vector3 targetPosition;
    private float currentSpeed;

    private void Awake()
    {
        initialScale = transform.localScale.x;
    }

    private void Start()
    {
        player = GameObject.Find("P_Player").transform;
        rigidBody = GetComponent<Rigidbody2D>();
        initialScale = bulletGameObject.transform.localScale.x;
        rigidBody.velocity = new Vector2(-1, 0);



    }

    private void Update()
    {
        if (wasHit)
        {
           // rigidBody.velocity = direction* Mathf.Lerp(maxSpeed, minSpeed, currentLerpTime);
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
            if (transform.localScale.x > minSize)
            {
                currentLerpTime += Time.deltaTime / maxSlowDuration;
                lerpTime += Time.deltaTime;

                float t = Mathf.Clamp01(currentLerpTime);

                float newScale = Mathf.Lerp(maxSize, minSize, t);
                float newSpeed = Mathf.Lerp(maxSpeed, minSpeed, t);

                transform.localScale = Vector3.one * newScale;
                rigidBody.velocity = rigidBody.velocity.normalized * newSpeed;
                currentVelocity = rigidBody.velocity;
            }
            else
            {
                inIdle = true;
                startingSlowDown = false;
                currentSpeed = currentVelocity.magnitude;
                //rigidBody.velocity = Vector2.zero;
            }
        }
        if (inIdle)
        {
            // Calculate the direction from the bullet to the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Set a target position with a slight delay
            targetPosition = player.position - directionToPlayer * 0.5f;

            // Calculate the desired velocity to reach the target position
            Vector2 desiredVelocity = (targetPosition - transform.position).normalized * maxSpeed;

            // Use SmoothDamp to smoothly adjust the bullet's velocity towards the desired velocity
            Vector2 smoothedVelocity = Vector2.SmoothDamp(rigidBody.velocity, desiredVelocity, ref currentVelocity, smoothTime);

            // Set the bullet's velocity to the smoothed velocity
            rigidBody.velocity = smoothedVelocity;

            // Gradually slow down the bullet
            rigidBody.velocity = Vector2.Lerp(smoothedVelocity, Vector2.zero, 1.5f * Time.deltaTime);

        }
        currentVelocity = rigidBody.velocity;
    }

    public void  hitEvent(Vector2 direction)
    {
        inIdle = false;
        wasHit = true;
        startingSlowDown = false;

        velocityTimeElapsed = 0;

        float clampedScale = Mathf.Clamp(transform.localScale.x * scaleUpFactor, minSize, maxSize);

        transform.localScale = Vector3.one * clampedScale;

        currentLerpTime = Mathf.InverseLerp(maxSize, minSize, clampedScale);    

        
        rigidBody.velocity = direction* Mathf.Lerp(maxSpeed, minSpeed, currentLerpTime);

        lerpTime = 0;

    }
}

