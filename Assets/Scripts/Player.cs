using UnityEngine;

public class Player : GroundableObject
{
    [SerializeField]
    float walkSpeed = 10;
    [SerializeField]
    float jumpSpeed = 15;
    [SerializeField]
    float jumpOffWallSpeed = 10;
    [SerializeField]
    float slideSpeed = 3;
    [SerializeField]
    float jumpOffWallDuration;
    [SerializeField]
    float gravityModifier;

    public Collider2D lavaCollider;
    public SpriteRenderer spriteRenderer;
    public Sprite standingUp;
    public Sprite movingRight;
    public Sprite movingLeft;

    float screenHalfWidth;
    float jumpedOffWallTimestamp;

    public event System.Action OnPlayerDeath;

    protected override void Start()
    {
        base.Start();
        screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
    }

    void Update()
    {
        var newVelocity = body.velocity;
        var horizontalInput = Input.GetAxisRaw("Horizontal");

        var justPressedJump = Input.GetKeyDown(KeyCode.UpArrow);

        var isGrounded = IsGrounded();
        var isCeilinged = IsCeilinged();

        // check for death
        if (isGrounded && isCeilinged || body.IsTouching(lavaCollider))
        {
            OnPlayerDeath?.Invoke();
            gameObject.SetActive(false);
            return;
        }

        var wallDirection = GetWallDirection();
        var justJumpedOffWall = Time.time - jumpedOffWallTimestamp < jumpOffWallDuration;
        var isHanging =
            !justJumpedOffWall
            && (
                (wallDirection < 0 && horizontalInput < 0)
                || (wallDirection > 0 && horizontalInput > 0)
            );

        bool nextToWallAndNotPushingAway = wallDirection * horizontalInput > 0;
        spriteRenderer.sprite = standingUp;

        // fixes sticking to walls if left/right is being held
        if (nextToWallAndNotPushingAway && !isGrounded && !justJumpedOffWall)
        {
            newVelocity.x = 0;
        }
        else
        {
            newVelocity.x = Mathf.Lerp(body.velocity.x, horizontalInput * walkSpeed, 0.05f);
            // if (horizontalInput == 1)
            // {
            //     spriteRenderer.sprite = movingRight;
            // }
            // else if (horizontalInput == -1)
            // {
            //     spriteRenderer.sprite = movingLeft;
            // }
        }

        if (justPressedJump)
        {
            if (isGrounded)
            {
                newVelocity.y = jumpSpeed;
            }
            else if (isHanging)
            {
                newVelocity = new Vector2(jumpOffWallSpeed * -wallDirection, jumpSpeed);
                jumpedOffWallTimestamp = Time.time;
            }
            Debug.LogWarning($"JUMPED {isGrounded} {isHanging} {newVelocity}");
        }
        else if (isHanging)
        {
            newVelocity.y = -slideSpeed;
        }

        body.velocity = newVelocity;

        if (body.position.x < -screenHalfWidth)
        {
            transform.position = new Vector2(screenHalfWidth, transform.position.y);
        }
        else if (body.position.x > screenHalfWidth)
        {
            transform.position = new Vector2(-screenHalfWidth, transform.position.y);
        }
    }

    void OnTriggerEnter2D(Collider2D triggerCollider)
    {
        print(triggerCollider);
        if (triggerCollider.CompareTag("Lava"))
        {
            Destroy(gameObject);
        }
    }
}
