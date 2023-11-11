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

    [SerializeField] private float intervalToAdjustTrajectory = 0.15f;
    private float intervalTimeElapsed;
    // Variables for velocity and movement
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

        //Idle();

        //PredictMovement(0);
       
        Idle(true);

        currentVelocity = rigidBody.velocity;

    }

    private void FixedUpdate()
    {
        CheckForBounce();
        
        intervalTimeElapsed += Time.deltaTime;
        if (intervalTimeElapsed>=intervalToAdjustTrajectory)
        {/*
            Vector3 newDir = pInstance.playerInstance.transform.position - transform.position;
            var angle = Mathf.Acos ((rigidBody.velocity.x*newDir.x + rigidBody.velocity.y* newDir.y )/ (rigidBody.velocity.magnitude * newDir.magnitude));
            var rotateVelocity = Quaternion.AngleAxis(angle, transform.forward);
            if (rotateVelocity != null)
            {
                rigidBody.velocity = rotateVelocity * currentVelocity;
                Debug.Log("CAll change trajectory");
            }*/
            intervalTimeElapsed = 0;

        }
        
        
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
        {
            RotateRocket();
            //rigidBody.velocity = transform.up * currentVelocity.magnitude;
        }
    }

    private void PredictMovement(float leadTimePercentage)
    {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);
        _standardPrediction = pInstance.playerInstance.transform.position + (Vector3)pInstance.playerInstance.GetRigibody().velocity * predictionTime;
    }

    private void RotateRocket_Homming()
    {
        Vector3 heading = pInstance.playerInstance.transform.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, heading);
        transform.rotation = rotation;

    }

    private void RotateRocket()
    {
        
        Vector3 targetDirection = (pInstance.playerInstance.transform.position - transform.position).normalized;
        
        Vector3 newDirection = Vector3.Lerp(rigidBody.velocity.normalized, targetDirection, Time.deltaTime * rotateSpeed);

        
        
        rigidBody.velocity =  rigidBody.velocity.magnitude * newDirection;

        // Ensure the bullet is always facing its direction of motion
        if (rigidBody.velocity != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, rigidBody.velocity);
        }
    }
    /*
     
    private void FixedUpdate() {
            _rb.velocity = transform.forward * _speed;

            var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.transform.position));

            PredictMovement(leadTimePercentage);

            AddDeviation(leadTimePercentage);

            RotateRocket();
        }
    
    private void PredictMovement(float leadTimePercentage)
    {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);
        _standardPrediction = _target.Rb.position + _target.Rb.velocity * predictionTime;
    }

    private void AddDeviation(float leadTimePercentage)
    {
        var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);

        var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

        _deviatedPrediction = _standardPrediction + predictionOffset;
    }

    */

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _standardPrediction);
    }


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
    #endregion
}
