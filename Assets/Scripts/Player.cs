using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float speed;

    public event System.Action OnPlayerDeath;

    Rigidbody2D body;
    float screenHalfWidth;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
    }

    void Update()
    {
        var velocity = body.velocity;
        velocity.x = Input.GetAxisRaw("Horizontal") * speed;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = speed;
        }
        body.velocity = velocity;

        if (body.position.x < -screenHalfWidth)
        {
            body.position = new Vector2(screenHalfWidth, transform.position.y);
        }
        else if (body.position.x > screenHalfWidth)
        {
            body.position = new Vector2(-screenHalfWidth, transform.position.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision);
    }
}
