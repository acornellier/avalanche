using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float jumpSpeed;
    [SerializeField]
    float jumpOffWallSpeed;
    [SerializeField]
    float slideSpeed;
    [SerializeField]
    float jumpOffWallDuration = 0.2f;
    [SerializeField]
    LayerMask jumpableMask;

    Rigidbody2D body;
    BoxCollider2D boxCollider;
    float screenHalfWidth;

    readonly float wallEpsilon = 0.01f;
    readonly float groundEpsilon = 0.05f;

    float jumpedOffWallTimestamp;

    public SpriteRenderer spriteRenderer;
    public Sprite standingUp;
    public Sprite movingRight;
    public Sprite movingLeft;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
    }

    void Update()
    {
        var newVelocity = body.velocity;
        var horizontalInput = Input.GetAxisRaw("Horizontal");

        var justPressedJump = Input.GetKeyDown(KeyCode.UpArrow);

        var isGrounded = IsGrounded();
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
        if (nextToWallAndNotPushingAway && !isGrounded && !justJumpedOffWall) {
            newVelocity.x = 0;
        }
        else {
            newVelocity.x = Mathf.Lerp(body.velocity.x, horizontalInput * walkSpeed, 0.05f);
            if (horizontalInput == 1) {
                spriteRenderer.sprite = movingRight;
            }
            else {
                spriteRenderer.sprite = movingLeft;
            }
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
        }
        else if (isHanging)
        {
            newVelocity.y = -slideSpeed;
        }

        body.velocity = newVelocity;

        if (body.position.x < -screenHalfWidth)
        {
            body.position = new Vector2(screenHalfWidth, transform.position.y);
        }
        else if (body.position.x > screenHalfWidth)
        {
            body.position = new Vector2(-screenHalfWidth, transform.position.y);
        }
    }

    bool IsGrounded()
    {
        var boxCastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            groundEpsilon,
            jumpableMask
        );

        var isGrounded = boxCastHit.collider != null;

        return isGrounded;
    }

    float GetWallDirection()
    {
        var leftBoxCastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.left,
            wallEpsilon,
            jumpableMask
        );

        if (leftBoxCastHit.collider != null)
            return -1;

        var rightBoxCastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.right,
            wallEpsilon,
            jumpableMask
        );

        if (rightBoxCastHit.collider != null)
            return 1;

        return 0;
    }
}
