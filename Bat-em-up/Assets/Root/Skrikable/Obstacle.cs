using UnityEngine;

public class Obstacle : MonoBehaviour, IStrikeable, IHiteable
{
    private Rigidbody2D rigidBody;

    [SerializeField] private EnemyMovement enemyMovement;
    private ScrollingEffect scrol;
    [SerializeField ]private InstanceLevelManager instanceLevel;

    private Vector2 screenBounds;
    private bool wasInScrrenBounds = false;
    private bool hasEnterScreen = false;

    private bool wasHit = false;

    private void Awake()
    {
        enemyMovement.Init(transform);
    }

    void Start()
    {

        scrol = instanceLevel.levelInstance.scrolType;
        Debug.Log(scrol.name );
        rigidBody = GetComponent<Rigidbody2D>();

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        screenBounds = new Vector2(camWidth, camHeight);
    }

    // Update is called once per frame
    void Update()
    {
        if (checkIsInScrren ()&& !hasEnterScreen)
        {
            hasEnterScreen = true;
            wasInScrrenBounds = true;
        }

        if (!checkIsInScrren () && wasInScrrenBounds)
            Destroy(gameObject);

    }

    private void FixedUpdate()
    {
        if (!wasHit)
            enemyMovement.Movement();

        scrol.ScrolEffect(transform);
    }

    private bool checkIsInScrren()
    {
        if ( transform.position.x < -screenBounds.x - 1
            || transform.position.x > screenBounds.x + 1
            || transform.position.y > screenBounds.y + 1
            || transform.position.y < -screenBounds.y -1)
                        return false;
        else return true;
    }

    public void Striking(Vector2 dir, float speed)
    {
        rigidBody.velocity = dir * speed;
        wasHit = true;
    }

    public void Hitting(Vector2 normal, float damage)
    {
        rigidBody.velocity = - normal * damage;
        wasHit = true;
    }
}
