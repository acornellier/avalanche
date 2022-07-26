using System;
using UnityEngine;

public class Player : GroundableObject
{
    [SerializeField] float walkSpeed = 10;
    [SerializeField] float jumpSpeed = 15;
    [SerializeField] float jumpOffWallSpeed = 10;
    [SerializeField] float slideSpeed = 3;
    [SerializeField] float jumpOffWallBufferTime;
    [SerializeField] float jumpOffWallDuration;
    [SerializeField] float shrinkRate = 1;
    [SerializeField] float expandRate = 0.1f;

    [SerializeField] Rigidbody2D body;

    public Collider2D lavaCollider;
    public SpriteRenderer spriteRenderer;
    public Sprite standingUp;
    public Sprite movingRight;
    public Sprite movingLeft;

    Vector3 _startingScale;
    float _screenHalfWidth;
    float _lastHangingTimestamp;
    float _jumpedOffWallTimestamp;

    float _melting;
    Renderer _rend;

    public event Action OnPlayerDeath;
    public event Action OnPlayerSquishDeath;

    protected override void Start()
    {
        base.Start();
        _rend = GetComponent<Renderer>();
        body = GetComponent<Rigidbody2D>();
        _screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        _startingScale = transform.localScale;
    }

    void Update()
    {
        var newVelocity = body.velocity;
        var horizontalInput = Input.GetAxisRaw("Horizontal");

        var justPressedJump = Input.GetKeyDown(KeyCode.UpArrow);

        var isGrounded = IsGrounded();
        var isCeilinged = IsCeilinged();

        var wallDirection = GetWallDirection();
        var justJumpedOffWall = Time.time - _jumpedOffWallTimestamp < jumpOffWallDuration;
        var isHanging =
            !justJumpedOffWall
            && (
                (wallDirection < 0 && horizontalInput < 0)
                || (wallDirection > 0 && horizontalInput > 0)
            );

        // check for lava death
        if (body.IsTouching(lavaCollider))
        {
            if (_melting > 1)
            {
                OnPlayerDeath?.Invoke();
                gameObject.SetActive(false);
                return;
            }

            _melting += 0.01f;
            _rend.material.color = Color.Lerp(Color.white, Color.black, _melting);
        }

        // check for squishing death
        if (isGrounded && isCeilinged)
        {
            var scaleChange = shrinkRate * Time.deltaTime * new Vector3(0, -1, 0);
            transform.localScale += scaleChange;

            if (transform.localScale.y < 0.1 * _startingScale.y)
            {
                OnPlayerSquishDeath?.Invoke();
                gameObject.SetActive(false);
                return;
            }
        }
        else if (transform.localScale.y < _startingScale.y)
        {
            var scaleChange = expandRate * Time.deltaTime * new Vector3(0, 1, 0);
            transform.localScale += scaleChange;
        }

        if (isHanging)
            _lastHangingTimestamp = Time.time;

        var nextToWallAndNotPushingAway = wallDirection * horizontalInput > 0;
        // spriteRenderer.sprite = standingUp;

        // fixes sticking to walls if left/right is being held
        if (nextToWallAndNotPushingAway && !isGrounded && !justJumpedOffWall)
            newVelocity.x = 0;
        else
            newVelocity.x = Mathf.Lerp(body.velocity.x, horizontalInput * walkSpeed, 0.05f);
        // if (horizontalInput == 1)
        // {
        //     spriteRenderer.sprite = movingRight;
        // }
        // else if (horizontalInput == -1)
        // {
        //     spriteRenderer.sprite = movingLeft;
        // }

        if (justPressedJump)
        {
            if (isGrounded)
            {
                newVelocity.y = jumpSpeed;
            }
            else if (isHanging || Time.time - _lastHangingTimestamp < jumpOffWallBufferTime)
            {
                newVelocity = new Vector2(jumpOffWallSpeed * -wallDirection, jumpSpeed);
                _jumpedOffWallTimestamp = Time.time;
            }
            // Debug.LogWarning($"JUMPED {isGrounded} {isHanging} {newVelocity}");
        }
        else if (isHanging)
        {
            newVelocity.y = -slideSpeed;
        }

        body.velocity = newVelocity;

        if (body.position.x < -_screenHalfWidth)
            transform.position = new Vector2(_screenHalfWidth, transform.position.y);
        else if (body.position.x > _screenHalfWidth)
            transform.position = new Vector2(-_screenHalfWidth, transform.position.y);
    }
}
