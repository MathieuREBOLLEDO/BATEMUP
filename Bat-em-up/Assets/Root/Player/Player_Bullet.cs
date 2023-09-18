using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class Player_Bullet : MonoBehaviour
{

    public float timeBeforeSlowDown = 0.25f;
    public float smoothTime = 0.5f;
    private Transform player;

    public float power = 15f;
    [Header("Speed")]
    
    public float minSpeed = 5f;
    public float maxSpeed = 30F;

    [Header ("Scale")]
    
    public float minSize = 0.5f;
    public float maxSize = 7f;
    public float scaleUpFactor = 1.4f;
    private Vector3 currentMaxScale;

    [Header ("Time")]
    public float maxSlowDuration = 15f;
    private float lerpTime;
    private float currentLerpTime;

    public float slowDownFactor = 0.1f;
    public float scaleDownFactor = 0.1f;


    private float velocityTimeElapsed;
    [SerializeField] GameObject bulletGameObject;
    [SerializeField] private bool startingSlowDown = true;
    [SerializeField] private Rigidbody2D rigidBody;
    private Vector2 currentVelocity = Vector2.zero;

    private float initialScale;
    private float targetScale = 0;
    private float originalVelocityMagnitude;

    private void Awake()
    {
        initialScale = transform.localScale.x;
    }

    private void Start()
    {
        player = GameObject.Find("P_Player").transform;
        rigidBody = GetComponent<Rigidbody2D>();
        initialScale = bulletGameObject.transform.localScale.x;
        targetScale = initialScale;
        originalVelocityMagnitude = rigidBody.velocity.magnitude;
        if (originalVelocityMagnitude == 0)
        {
            originalVelocityMagnitude = minSpeed;
        }

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
                currentLerpTime + = Time.deltaTime;
                float t = currentLerpTime / lerpTime;
                transfrom.localScale = Vector3.one * Mathf(minSize, maxSize, t);
                //rigidBody.velocity -= rigidBody.velocity.normalized * slowDownFactor * Time.deltaTime;
                //targetVelocity = rigidBody.velocity;
            }
            else
            {
                targetVelocity = (player.position - (Vector3)rigidBody.position).normalized * minSpeed;
            }

            /*  
            // Update target scale to match slow down
            float normalizedVelocityMagnitude = Mathf.Clamp01(rigidBody.velocity.magnitude / originalVelocityMagnitude);

            targetScale = Mathf.Lerp(currentMaxScale.x , initialScale, normalizedVelocityMagnitude);

            // Smooth scaling


            //float smoothedScale = Mathf.Lerp(bulletGameObject.transform.localScale.x, targetScale, Time.deltaTime / smoothTime);
            bulletGameObject.transform.localScale = Vector3.one * targetScale;
            
            */
            // Apply smoothed velocity
            rigidBody.velocity = Vector2.SmoothDamp(
                rigidBody.velocity,
                targetVelocity,
                ref currentVelocity,
                smoothTime
            );
        }
    }

    public void hitEvent(Vector3 direction)
    {
       // Debug.Log("Call Hit");
        startingSlowDown = false;
        velocityTimeElapsed = 0;

        CalculateLerpTime();
        //lerpTime = maxSlowDuration * ()
        
        //GetComponent<Rigidbody2D>().velocity = direction * (accPower + GetComponent<Rigidbody2D>().velocity.magnitude);
        //currentMaxScale = transform.localScale * scaleUpFactor;
        //transform.localScale = currentMaxScale;
    }

    private void CalculateLerpTime()
    {
        float clampedScale = Mathf.Clamp(transform.localScale.x * scaleUpFactor, minSize, maxSize);
        lerpTime = maxSlowDuration * (clampedA/ maxSize)
    }
}

