using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;


// Alias the UnityEngine.Debug class as Debug to avoid conflicts with System.Diagnostics.Debug

public class Player : MonoBehaviour
{
    [SerializeField] private GetPlayerInstance instPlayer;
    [SerializeField] private GetBulletInstance bInstance;
    /*
    [Header("Key Binding")]
    public KeyCode shotButton = KeyCode.LeftShift;
    public KeyCode left = KeyCode.LeftArrow;
    public KeyCode right = KeyCode.RightArrow;
    public KeyCode up = KeyCode.UpArrow;
    public KeyCode down = KeyCode.DownArrow;
    */

    [Header("Stats_Player")]
    public int lifePoints = 5;
    public float invulnerabilityDuration = 0.5f;
    [SerializeField] private float movementSpeed = 5f;


    public float attackRate = 0.5f;
    public float animationSpeed = 1.75f;
    public float maxBounceAngle = 45f;

    [Header("Weapon Component")]
    [SerializeField] private GameObject weaponGO;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private Animator weaponSwingFXAnimator;

    [Header("Boolean States")]
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool canBeDamaged = true;
    [SerializeField] private bool isAttacking = false;
    private int attackCollideValue = 0;
    [SerializeField] private bool isHurt = false;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isFlickering = false;
    //public float flickerDuration = 0.2f;
    public float flickerSpeed = 10f;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color initColor;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rigidBody;

    [SerializeField] private GameObject gizmoArrow;

    [Header("Camera")]
    [SerializeField] private float topBottomOffset = 0.5f;
    [SerializeField] private float rightOffset = 5f;
    [SerializeField] private float leftOffset = 0.5f;
    private Vector2 screenBounds;

    [Header("Events")]
    public UnityEvent onHurt;
    public UnityEvent onDeath;
    public UnityEvent onRespawn;

    float fireTimer;
    float attackTimer;

    private void Awake()
    {
        instPlayer.playerInstance = this;
    }

    void Start()
    {
        // Calculate screen bounds
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        screenBounds = new Vector2(camWidth, camHeight);
        initColor = spriteRenderer.material.color;
        //weaponSwingFXAnimator.speed = animationSpeed;
    }

    void Update()
    {
        Vector2 initialDirection = ( bInstance.bulletInstance.transform.position- transform.position ).normalized;

        float angle = Vector2.SignedAngle(transform.up, initialDirection);

        // Get the sign of the angle
        float sign = Mathf.Sign(angle);

        // Clamp the absolute angle to the specified range
        float clampedAngle = Mathf.Clamp(Mathf.Abs(angle), 0, maxBounceAngle);

        // Calculate the final direction based on the clamped angle
        Vector2 finalDirection = Quaternion.Euler(0, 0, sign * clampedAngle) * transform.up;
        gizmoArrow.transform.up = finalDirection;

        // Handle player's movement
        HandleMovement();

        // Handle thruster animations
        HandleAnimationMovement();

        // Handle player's attacks
        HandleAttack();

        if (Input.GetButton("Jump"))
        {
            Hurt();
        }
    }

    void LateUpdate()
    {
        // Ensure the player stays within screen bounds
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x + leftOffset, screenBounds.x - rightOffset);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y + topBottomOffset, screenBounds.y - topBottomOffset);
        transform.position = newPosition;
    }


    private void HandleMovement()
    {
        // Handle player's movement based on input
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector2 inputVector = new Vector2(inputX, inputY);

        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }

        rigidBody.velocity = inputVector * movementSpeed;
    }

    private void HandleAnimationMovement()
    {
        animator.SetFloat("HorizMovement", Input.GetAxis("Horizontal"));

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Call event for player's attack when colliding with certain objects
        if (collision.GetComponent<MonoBehaviour>() as IStrikeable != null && isAttacking)
        {
            CallEventAttack(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<MonoBehaviour>() as IStrikeable != null && isAttacking)
        {
            CallEventAttack(collision);
        }
    }

    private void HandleAttack()
    {
        // Handle player's attack
        attackTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") || Input.GetButton("Fire2"))
        {
            if (attackTimer >= attackRate)
            {
                isAttacking = true;
                // Activate attack animation and effect
                weaponSwingFXAnimator.SetTrigger("Attacking");

                Invoke("EndAttack", 0.25f);

                attackTimer = 0f;
            }
        }
    }

    private void CallEventAttack(Collider2D collision)
    {

        if (attackCollideValue == 0)
        {

            //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Vector2 direction = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y).normalized;

           // Vector2 direction = (Vector2)(collision.transform.position - transform.position).normalized;

            Vector2 initialDirection = (bInstance.bulletInstance.transform.position - transform.position).normalized;

            float angle = Vector2.SignedAngle(transform.up, initialDirection);

            // Get the sign of the angle
            float sign = Mathf.Sign(angle);

            // Clamp the absolute angle to the specified range
            float clampedAngle = Mathf.Clamp(Mathf.Abs(angle), 0, maxBounceAngle);

            // Calculate the final direction based on the clamped angle
            Vector2 finalDirection = Quaternion.Euler(0, 0, sign * clampedAngle) * transform.up;


            attackCollideValue++;
            // Trigger hit event on the collided bullet
            collision.GetComponent<IStrikeable>().Striking(finalDirection);
            Time.timeScale = 0.1f;
            Invoke("EndImpactEffect", Time.deltaTime);

        }


    }

    private void EndImpactEffect()
    {
        // Reset time scale to normal
        Time.timeScale = 1f;
        CameraShaker.Invoke();
    }

    private void EndAttack()
    {
        // Deactivate attack animation and reset attack state
        isAttacking = false;
        attackCollideValue = 0;
    }

    public void Hurt()
    {
        if (!isHurt)
        {
            isHurt = true;
            lifePoints--;
            onHurt.Invoke();
            if (lifePoints < 0)
                Die();
            if (!isFlickering)
            {
                isFlickering = true;
                StartCoroutine(Flicker());
            }

            Invoke("StopInvulnerable", invulnerabilityDuration);

        }

    }

    IEnumerator Flicker()
    {
        for (float t = 0; t < invulnerabilityDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Sin(flickerSpeed * t) > 0 ? 0 : 1;
            Color flickerColor = new Color(initColor.r, initColor.g, initColor.b, alpha);
            spriteRenderer.material.color = flickerColor;
            yield return null;
        }
        spriteRenderer.material.color = initColor;
        isFlickering = false;
    }

    private void StopInvulnerable()
    {
        isHurt = false;
    }

    private void Die()
    {
        //GetComponent<BulletReceiver>().enabled = false;

        onDeath.Invoke();
    }
}
