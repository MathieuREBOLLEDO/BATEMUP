using UnityEngine;

public class PlayerBullet : MonoBehaviour, IStrikeable
{
    //public static Player_Bullet instance_PBullet;

    [SerializeField] private GetPlayerInstance pInstance;
    [SerializeField] private GetBulletInstance bInstance;

    // Variables for bullet state and behavior
    public bool inIdle = true;
    public bool wasHit = false;
    public bool startingSlowDown = false;
    public float timeBeforeSlowDown = 0.25f;
    public float smoothTime = 0.1f;

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


    [Header("PREDICTION")]
    [SerializeField] private float _maxDistancePredict = 100;
    [SerializeField] private float _minDistancePredict = 5;
    [SerializeField] private float _maxTimePrediction = 5;
    private Vector3 _standardPrediction, _deviatedPrediction;

    [Header("Time")]
    public float maxSlowDuration = 15f;
    public float lerpTime;
    private float currentLerpTime;

    // Variables for velocity and movement
    private float velocityTimeElapsed;

    [SerializeField] private Rigidbody2D rigidBody;
    public Vector2 currentVelocity = Vector2.zero;


    [Header("VFX")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private TrailRenderer prefabTrail;

    private TrailRenderer trail;

    private Vector3 targetPosition;

    #region Init

    private void Awake()
    {

        bInstance.bulletInstance = this;
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
        if (wasHit)
        {
            // Handle behavior when the bullet was hit by player
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
            SizeNSlowDown();
        }

        Idle();
/*
        PredictMovement(0);
        RotateRocket();
        Idle(true);
*/

        currentVelocity = rigidBody.velocity;
    }

    private void FixedUpdate()
    {
        
    }

    #region States
    private void Idle()
    {
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
    }

    private void Idle(bool a)
    {
        if (inIdle)
            rigidBody.velocity = transform.up * minSpeed;

    }

    private void PredictMovement(float leadTimePercentage)
    {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);
        _standardPrediction = pInstance.playerInstance.transform.position + (Vector3)pInstance.playerInstance.GetRigibody().velocity * predictionTime;
    }

    private void RotateRocket()
    {
        Vector2 heading = pInstance.playerInstance.transform.position - transform.position;

        //heading.Normalize();

        float rotateAmout = Vector3.Cross(heading.normalized, transform.up).z;

        rigidBody.angularVelocity = -rotateAmout * rotateSpeed;

    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
    }
    */

    private void SizeNSlowDown()
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
        }
    }

    public void Striking(Vector2 direction, float speed)
    {
        // var speed not use for this element

        inIdle = false;
        wasHit = true;
        startingSlowDown = false;

        lerpTime = 0;
        velocityTimeElapsed = 0;

        float clampedScale = Mathf.Clamp(transform.localScale.x * scaleUpFactor, minSize, maxSize);
        transform.localScale = Vector3.one * clampedScale;

        currentLerpTime = Mathf.InverseLerp(maxSize, minSize, clampedScale);
        rigidBody.velocity = direction * Mathf.Lerp(maxSpeed, minSpeed, currentLerpTime);
        trail.time = Mathf.Lerp(trailMaxSize, trailMinSize, currentLerpTime);

    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MonoBehaviour>() as IHiteable != null)
        {
            Vector2 collisionNormal = (transform.position - collision.transform.position).normalized;
            collision.GetComponent<IHiteable>().Hitting(collisionNormal, transform.localScale.x);
            GameObject.Instantiate(explosion, transform.position, Quaternion.identity);

            rigidBody.velocity = collisionNormal * rigidBody.velocity;
            //rigidBody.velocity = Vector2.Reflect(currentVelocity, collisionNormal);
        }
    }


}
