using UnityEngine;

// Alias the UnityEngine.Debug class as Debug to avoid conflicts with System.Diagnostics.Debug

public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("Stats_Player")]
    public int lifePoints = 5;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float maxSpeed = 10f;

    [Header("Weapons")]
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float fireDamage = 10f;
    [SerializeField] private float bulletSpeed = 15f;

    [SerializeField] private float attackRate = 0.5f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackPower = 15f;
    private float nextAttack = 0f;

    [Header("Weapon Component")]
    [SerializeField] private GameObject weaponGO;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private Animator weaponSwingFXAnimator;

    [Header("Boolean States")]
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool canBeDamaged = true;
    [SerializeField] private bool isAttacking = false;
    private int attackCollideValue = 0;
    [SerializeField] private bool isHit = false;
    [SerializeField] private bool isDead = false;

    [Header("Components")]
    [SerializeField] private Animator[] vehicleThrusters;
    [SerializeField] private GameObject muzzleLeft;
    [SerializeField] private GameObject muzzleRight;
    [SerializeField] private GameObject muzzleCenter;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private PolygonCollider2D polygonCollider;

    [Header("Camera")]
    [SerializeField] private float topBottomOffset = 0.5f;
    [SerializeField] private float rightOffset = 5f;
    [SerializeField] private float leftOffset = 0.5f;
    private Vector2 screenBounds;

    float fireTimer;
    float attackTimer;

    private void Awake()
    {
        // Check if an instance of Player already exists.
        if (instance == null)
        {
            // If not, set this instance as the Singleton instance.
            instance = this;

            // Ensure that this GameObject persists across scenes.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this duplicate.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Calculate screen bounds
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        screenBounds = new Vector2(camWidth, camHeight);
    }

    void Update()
    {
        // Handle player's movement
        HandleMovement();

        // Handle thruster animations
        HandleThrusterAnimations();

        // Handle player's attacks
        HandleAttack();
    }

    void LateUpdate()
    {
        // Ensure the player stays within screen bounds
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x + leftOffset, screenBounds.x - rightOffset);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y + topBottomOffset, screenBounds.y - topBottomOffset);
        transform.position = newPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collision with objects
        if (collision.gameObject.GetComponent<CircleCollider2D>())
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Call event for player's attack when colliding with certain objects
        if (collision.gameObject.CompareTag("Bouncing_Bullet") && isAttacking)
        {
            CallEventAttack(collision);
        }
        
        else if( !collision.gameObject.CompareTag("Bouncing_Bullet") && polygonCollider.IsTouching(collision))
        {
            lifePoints--;
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bouncing_Bullet") && isAttacking)
        {
            CallEventAttack(collision);
        }
    }

    private void HandleMovement()
    {
        // Handle player's movement based on input
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        rigidBody.velocity = new Vector2(inputX * maxSpeed, inputY * maxSpeed);
    }

    private void HandleThrusterAnimations()
    {
        // Update thruster animations based on player's input
        bool isMovingForward = Input.GetAxis("Horizontal") > 0;
        bool isMovingBackward = Input.GetAxis("Horizontal") < 0;
        bool isMovingDown = Input.GetAxis("Vertical") < 0;
        bool isMovingUp = Input.GetAxis("Vertical") > 0;

        // Set animation parameters for vehicle thrusters
        vehicleThrusters[0].SetBool("Moving", isMovingForward);
        vehicleThrusters[1].SetBool("Moving", isMovingForward);
        vehicleThrusters[2].SetBool("Moving", isMovingBackward);
        vehicleThrusters[3].SetBool("Moving", isMovingBackward);
        vehicleThrusters[4].SetBool("Moving", isMovingDown);
        vehicleThrusters[5].SetBool("Moving", isMovingUp);
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

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y).normalized;

        if (collision.GetComponent<Player_Bullet>() && attackCollideValue == 0)
        {
            attackCollideValue++;
            // Trigger hit event on the collided bullet
            collision.GetComponent<Player_Bullet>().hitEvent(direction);
            Time.timeScale = 0.1f;
            Invoke("EndImpactEffect", Time.deltaTime);
        }

    }

    private void EndImpactEffect()
    {
        // Reset time scale to normal
        Time.timeScale = 1f;
    }

    private void EndAttack()
    {
        // Deactivate attack animation and reset attack state
        isAttacking = false;
        attackCollideValue = 0;
    }
}
