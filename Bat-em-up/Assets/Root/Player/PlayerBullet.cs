using UnityEngine;

enum BulletStates
{
    Idle,
    SlowDown,
    Hitting,
    WasStrike
}

public class PlayerBullet : MonoBehaviour, IStrikeable
{
    //public static Player_Bullet instance_PBullet;

    [SerializeField] private GetPlayerInstance pInstance;
    [SerializeField] private GetBulletInstance bInstance;

    [SerializeField] private BulletStates bState;

    // Variables for bullet properties
    public float power = 15f;
    [Header("Speed")]
    [SerializeField] public float minSpeed = 5f;
    [SerializeField] public float maxSpeed = 30F;
    public float rotateSpeed = 200f;

    [Header("Scale")]
    public float minSize = 0.5f;
    public float maxSize = 7f;
    public float scaleUpFactor = 1.4f;

    public float trailMinSize = 0.1f;
    public float trailMaxSize = 0.5f;

    [Header("Time")]
    public float maxSlowDuration = 15f;
    public float timeBeforeSlowDown = 0.25f;
    public float smoothTime = 0.1f;
    public float lerpTime;
    private float currentLerpTime;

    private float velocityTimeElapsed;

    [SerializeField] private Rigidbody2D rigidBody;
    public Vector2 currentVelocity = Vector2.zero;


    [Header("VFX")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private TrailRenderer prefabTrail;
    [SerializeField] private ParticleSystem bounceParticle;

    private TrailRenderer trail;

    private Vector3 targetPosition;

    private Vector2 screenBounds;

    #region Init

    private void Awake()
    {
        bInstance.bulletInstance = this;
        GetScreenBounds();
    }

    private void Start()
    {
        // Find the player's transform by name
        trail = GameObject.Instantiate(prefabTrail, transform.position, Quaternion.identity);

        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(-1, 0);
    }
    #endregion

    private void Update()
    {
        trail.transform.position = transform.position;

        currentVelocity = rigidBody.velocity;

        switch (bState)
        {
            case BulletStates.Idle:
                currentVelocity = Idle();
                break;

            case BulletStates.WasStrike:
                TimeCheckBeforeSlowDown();
                currentVelocity = rigidBody.velocity;
                break;

            case BulletStates.SlowDown:
                currentVelocity = SizeNSlowDown();
                break;

        }

        currentVelocity = MoveTowardPlayer();

        rigidBody.velocity = currentVelocity;
    }

    private void FixedUpdate()
    {
        CheckForBounce();

    }

    #region States

    private void TimeCheckBeforeSlowDown()
    {
        velocityTimeElapsed += Time.deltaTime;

        if (velocityTimeElapsed >= timeBeforeSlowDown)        
            bState = BulletStates.SlowDown;
        
    }

    private Vector2 Idle()
    {
        Vector2 tmpVelocity = currentVelocity;

        // Handle behavior when the bullet is in idle state
        Vector3 directionToPlayer = (pInstance.playerInstance.transform.position - transform.position).normalized;
        targetPosition = pInstance.playerInstance.transform.position - directionToPlayer * 0.05f;
        Vector2 desiredVelocity = (targetPosition - transform.position).normalized * maxSpeed;
        Vector2 smoothedVelocity = Vector2.SmoothDamp(rigidBody.velocity, desiredVelocity, ref currentVelocity, smoothTime);
        tmpVelocity = smoothedVelocity;
        tmpVelocity = Vector2.Lerp(smoothedVelocity, Vector2.zero, 1.25f * Time.deltaTime);

        return tmpVelocity;
    }
    private Vector2 SizeNSlowDown()
    {
        Vector2 tmpVelocity = currentVelocity;
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
            tmpVelocity = rigidBody.velocity.normalized * newSpeed;
            trail.time = newTrailSize;
        }

        else
        {
            bState = BulletStates.Idle;
        }

        return tmpVelocity;
    }

    private Vector2 MoveTowardPlayer()
    {
        Vector2 playerPos = new Vector2(pInstance.playerInstance.transform.position.x, pInstance.playerInstance.transform.position.y);

        float tmpMagnitude = currentVelocity.magnitude;
        Vector2 currentDirection = currentVelocity.normalized;
        Vector2 dirToPlayer = (playerPos - (Vector2)transform.position).normalized;

        Vector2 newDirection = new Vector2(Mathf.Lerp(currentDirection.x, dirToPlayer.x, Time.deltaTime * rotateSpeed), currentDirection.y);

        return newDirection * tmpMagnitude;

    }

    public void Striking(Vector2 direction, float speed)
    {
        // var speed not use for this element

        bState = BulletStates.WasStrike;


        lerpTime = 0;
        velocityTimeElapsed = 0;

        float clampedScale = Mathf.Clamp(transform.localScale.x * scaleUpFactor, minSize, maxSize);
        transform.localScale = Vector3.one * clampedScale;

        currentLerpTime = Mathf.InverseLerp(maxSize, minSize, clampedScale);
        rigidBody.velocity = direction * Mathf.Lerp(maxSpeed, minSpeed, currentLerpTime);
        trail.time = Mathf.Lerp(trailMaxSize, trailMinSize, currentLerpTime);

    }

    #region Bounce
    private void GetScreenBounds()
    {
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        screenBounds = new Vector2(camWidth, camHeight);
    }

    private void ClampPosition(Vector3 newPos)
    {
        newPos.x = Mathf.Clamp(newPos.x, -screenBounds.x, screenBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, -screenBounds.y, screenBounds.y);
        transform.position = newPos;
    }

    private void CheckForBounce()
    {
        Vector3 newPosition = transform.position;
        if (newPosition.x < -screenBounds.x || newPosition.x > screenBounds.x)
        {
            // Reflect the ball's velocity off the screen edge
            rigidBody.velocity = new Vector2(-rigidBody.velocity.x, rigidBody.velocity.y);
            Instantiate(bounceParticle, newPosition, Quaternion.identity);
        }

        if (newPosition.y < -screenBounds.y || newPosition.y > screenBounds.y)
        {
            // Reflect the ball's velocity off the screen edge
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, -rigidBody.velocity.y);
            Instantiate(bounceParticle, newPosition, Quaternion.identity);
        }
        ClampPosition(newPosition);
    }

    private void BounceOnEnnemy(Collider2D other, Vector2 normal)
    {
        GameObject.Instantiate(bounceParticle, transform.position, Quaternion.identity);
        rigidBody.velocity = normal * currentVelocity.magnitude;

    }
    #endregion
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 collisionNormal = (transform.position - collision.transform.position).normalized;
        if (collision.GetComponent<MonoBehaviour>() as IHiteable != null)
        {
            BounceOnEnnemy(collision, collisionNormal);
            collision.GetComponent<IHiteable>().Hitting(collisionNormal, transform.localScale.x);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)currentVelocity.normalized * currentVelocity.magnitude / 10);
    }
}
