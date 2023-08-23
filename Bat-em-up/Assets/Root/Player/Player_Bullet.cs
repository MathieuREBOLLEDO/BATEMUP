using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : MonoBehaviour
{
    public float timeBeforeSlowDown = 2.0f;
    public float smoothTime = 0.5f;
    private Transform player;
    public float minSize = 1f;
    public float minSpeed = 5f;
    public float slowDownFactor = 0.1f;
    public float scaleDownFactor = 0.1f;

    private float velocityTimeElapsed;
    [SerializeField] private bool startingSlowDown;
    [SerializeField] private Rigidbody2D rigidBody;
    private Vector2 currentVelocity = Vector2.zero;

    private float initialScale;
    private float targetScale;
    private float originalVelocityMagnitude;

    private void Start()
    {
        player = GameObject.Find("P_Player").transform;
        rigidBody = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale.x;
        targetScale = initialScale;
        originalVelocityMagnitude = rigidBody.velocity.magnitude;
    }

    private void Update()
    {
        if (!startingSlowDown)
        {
            velocityTimeElapsed += Time.deltaTime;

            if (velocityTimeElapsed >= timeBeforeSlowDown)
            {
                startingSlowDown = true;
            }
        }

        if (startingSlowDown)
        {
            // Slow down velocity
            Vector2 targetVelocity;

            if (rigidBody.velocity.magnitude > minSpeed)
            {
                rigidBody.velocity -= rigidBody.velocity.normalized * slowDownFactor * Time.deltaTime;
                targetVelocity = rigidBody.velocity;
            }
            else
            {
                targetVelocity = (player.position - (Vector3)rigidBody.position).normalized * minSpeed;
            }

            // Update target scale to match slow down
            float normalizedVelocityMagnitude = Mathf.Clamp01(rigidBody.velocity.magnitude / originalVelocityMagnitude);
            targetScale = Mathf.Lerp(minSize, initialScale, normalizedVelocityMagnitude);

            // Smooth scaling
            float smoothedScale = Mathf.Lerp(transform.localScale.x, targetScale, Time.deltaTime / smoothTime);
            transform.localScale = Vector3.one * smoothedScale;

            // Apply smoothed velocity
            rigidBody.velocity = Vector2.SmoothDamp(
                rigidBody.velocity,
                targetVelocity,
                ref currentVelocity,
                smoothTime
            );
        }
    }
}

