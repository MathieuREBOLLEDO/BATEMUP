using UnityEngine;

public class Obstacle : MonoBehaviour, IStrikeable
{
    private Rigidbody2D rigidBody;

    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private ScrollingEffect scrol;

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
        if ( transform.position.x < -screenBounds.x
            || transform.position.x > screenBounds.x
            || transform.position.y > screenBounds.y
            || transform.position.y < -screenBounds.y)
                        return false;
        else return true;
    }

    public void Striking(Vector2 dir, float speed)
    {
        rigidBody.velocity = dir * speed;
        wasHit = true;
    }
}
