using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats_Player")]
    [SerializeField] private float lifePoints = 100;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float maxSpeed = 10f;

    [Header("Weapons")]
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float fireDamage = 10f;
    [SerializeField] private float bulletSpeed = 15f;

    [SerializeField] private float attackRate = 0.5f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackPower = 15f;
    [SerializeField] float scaleUpFactor = 4f;
    private float nextAttack = 0f;

    [Header("Weapon Component")]
    [SerializeField] private GameObject weaponGO;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private Animator weaponSwingFXAnimator;

    [Header("Boolean States")]
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool canBeDamaged = true;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isHit = false;
    [SerializeField] private bool isDead = false;

    [Header("Components")]
    [SerializeField] private Animator[] vehicleThrusters;
    [SerializeField] private GameObject muzzleLeft;
    [SerializeField] private GameObject muzzleRight;
    [SerializeField] private GameObject muzzleCenter;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    private PolygonCollider2D polygonCollider;

    [Header("Camera")]
    [SerializeField] private float topBottomOffset = 0.5f;
    [SerializeField] private float rightOffset = 5f;
    [SerializeField] private float leftOffset = 0.5f;
    private Vector2 screenBounds;

    float fireTimer;
    float attackTimer;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        weaponGO.SetActive(false);
        //mainCamera = Camera.main;

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        screenBounds = new Vector2(camWidth, camHeight);
    }

    void Update()
    {
        HandleMovement();
        HandleThrusterAnimations();
        HandleAttack();
        
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(mousePos);
    }

    void LateUpdate()
    {
        Vector3 newPosition = transform.position;

        newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x + leftOffset, screenBounds.x - rightOffset);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y + topBottomOffset, screenBounds.y - topBottomOffset);
        transform.position = newPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bouncing_Bullet"))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mousePos-transform.position).normalized;
            //Vector3 direction = (collision.transform.position - transform.position).normalized;

            //collision.GetComponent<Rigidbody2D>().velocity = direction * (attackPower + collision.GetComponent<Rigidbody2D>().velocity.magnitude);
            //collision.transform.localScale = collision.transform.localScale * scaleUpFactor;

            //collision.gameObject.layer = LayerMask.NameToLayer("Bullet_Player");

            if ( collision.GetComponent<Player_Bullet>())
            {
                collision.GetComponent<Player_Bullet>().hitEvent(direction);
            }
        }
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        rigidBody.velocity = new Vector2(inputX * maxSpeed, inputY * maxSpeed);
    }

    private void HandleThrusterAnimations()
    {
        bool isMovingForward = Input.GetAxis("Horizontal") > 0;
        bool isMovingBackward = Input.GetAxis("Horizontal") < 0;
        bool isMovingDown = Input.GetAxis("Vertical") < 0;
        bool isMovingUp = Input.GetAxis("Vertical") > 0;

        vehicleThrusters[0].SetBool("Moving", isMovingForward);
        vehicleThrusters[1].SetBool("Moving", isMovingForward);
        
        vehicleThrusters[2].SetBool("Moving", isMovingBackward);
        vehicleThrusters[3].SetBool("Moving", isMovingBackward);
        vehicleThrusters[4].SetBool("Moving", isMovingDown);
        vehicleThrusters[5].SetBool("Moving", isMovingUp);
        
    }

    private void HandleAttack()
    {
        attackTimer += Time.deltaTime;

        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            if (attackTimer >= attackRate)
            {
                isAttacking = true;
                weaponGO.SetActive(true);
                //weaponAnimator.SetTrigger("Attacking");
                weaponSwingFXAnimator.SetTrigger("Attacking");
                Invoke("EndAttack", 0.25f);


                attackTimer = 0f;
            }
        }
    }

    private void EndAttack()
    {
        weaponGO.SetActive(false);
        isAttacking = false;
    }
}