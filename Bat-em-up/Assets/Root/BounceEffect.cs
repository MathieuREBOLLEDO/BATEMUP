using UnityEngine;

public class BounceEffect : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 screenBounds;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Calculate screen bounds based on camera view
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        screenBounds = new Vector2(camWidth, camHeight);
        //Debug.Log("Screen Bounds : " + screenBounds);
    }

    void Update()
    {
        // Move the ball based on its velocity
        transform.position += (Vector3)rb.velocity * Time.deltaTime;

        // Check if the ball is outside the screen bounds
        Vector3 newPosition = transform.position;
        if (newPosition.x < -screenBounds.x || newPosition.x > screenBounds.x)
        {
            // Reflect the ball's velocity off the screen edge
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }

        if (newPosition.y < -screenBounds.y || newPosition.y > screenBounds.y)
        {
            // Reflect the ball's velocity off the screen edge
            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
        }

        // Clamp the ball's position to screen bounds
        newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x, screenBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y, screenBounds.y);
        transform.position = newPosition;
    }
}
