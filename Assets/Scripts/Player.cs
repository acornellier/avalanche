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
    float jumpOffWallBufferTime;
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
    float lastHangingTimestamp;
    float jumpedOffWallTimestamp;

    float melting = 0f;
    Renderer rend;

    public event System.Action OnPlayerDeath;

    protected override void Start()
    {
        rend = GetComponent<Renderer> ();
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

        // check for lava death
        if (body.IsTouching(lavaCollider))
        {
            if (melting > 1) {
                OnPlayerDeath?.Invoke();
                gameObject.SetActive (false);
                return;
            }
            melting += 0.01f;
            rend.material.color = Color.Lerp(Color.white, Color.black, melting);
        }

        // check for squishing death
        if (isGrounded && isCeilinged)
        {
            Vector3 scaleChange = new Vector3(0, -0.005f, 0);
            transform.localScale += scaleChange;
            if (transform.localScale.y < 0.10f){
                OnPlayerDeath?.Invoke();
                gameObject.SetActive (false);
                return;
            }
        }
        else if (isCeilinged){
            Vector3 scaleChange = new Vector3(-0.3f, -0.3f, -0.3f);
            transform.localScale += scaleChange;
            newVelocity.y = jumpSpeed;
        }
        else if (transform.localScale.x < 0.78f) {  
            Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);
            transform.localScale += scaleChange;
        }
        else if (transform.localScale.y < 0.95f) {  
            Vector3 scaleChange = new Vector3(0, 0.01f, 0);
            transform.localScale += scaleChange;
        }

        var wallDirection = GetWallDirection();
        var justJumpedOffWall = Time.time - jumpedOffWallTimestamp < jumpOffWallDuration;
        var isHanging =
            !justJumpedOffWall
            && (
                (wallDirection < 0 && horizontalInput < 0)
                || (wallDirection > 0 && horizontalInput > 0)
            );

        if (isHanging)
            lastHangingTimestamp = Time.time;

        bool nextToWallAndNotPushingAway = wallDirection * horizontalInput > 0;
        // spriteRenderer.sprite = standingUp;

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
            else if (isHanging || (Time.time - lastHangingTimestamp < jumpOffWallBufferTime))
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
}
