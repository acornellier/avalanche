using System;
using System.Collections;
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
    [SerializeField] ParticleSystem explosion;
    [SerializeField] Collider2D lavaCollider;

    Renderer _renderer;
    Vector3 _startingScale;
    float _screenHalfWidth;
    float _lastHangingTimestamp;
    float _jumpedOffWallTimestamp;
    float _timeSpentMelting;
    bool _isDead;

    public event Action OnPlayerDeath;

    protected override void Start()
    {
        base.Start();
        _renderer = GetComponent<Renderer>();
        _screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        _startingScale = transform.localScale;
    }

    void Update()
    {
        if (_isDead) return;

        var newVelocity = body.velocity;
        var horizontalInput = Input.GetAxisRaw("Horizontal");

        var isGrounded = IsGrounded();
        var isCeilinged = IsCeilinged();

        var wallDirection = GetWallDirection();
        var justJumpedOffWall = Time.time - _jumpedOffWallTimestamp < jumpOffWallDuration;
        var isHanging =
            !isGrounded
            && !justJumpedOffWall
            && body.velocity.y <= 0
            && (
                (wallDirection < 0 && horizontalInput < 0)
                || (wallDirection > 0 && horizontalInput > 0)
            );

        // check for lava death
        if (body.IsTouching(lavaCollider))
        {
            if (_timeSpentMelting > 1)
            {
                StartCoroutine(Explode());
                return;
            }

            _timeSpentMelting += Time.deltaTime;
            _renderer.material.color = Color.Lerp(Color.white, Color.black, _timeSpentMelting);
        }

        // check for squishing death
        if (isGrounded && isCeilinged)
        {
            var scaleChange = shrinkRate * Time.deltaTime * new Vector3(0, -1, 0);
            transform.localScale += scaleChange;

            if (transform.localScale.y < 0.1 * _startingScale.y)
            {
                StartCoroutine(Explode());
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

        // fixes sticking to walls if left/right is being held
        if (nextToWallAndNotPushingAway && !isGrounded && !justJumpedOffWall)
            newVelocity.x = 0;
        else
            newVelocity.x = Mathf.Lerp(body.velocity.x, horizontalInput * walkSpeed, 0.05f);

        if (Input.GetKeyDown(KeyCode.UpArrow))
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

    IEnumerator Explode()
    {
        _isDead = true;
        explosion.Play();
        while (explosion.isPlaying) yield return null;
        OnPlayerDeath?.Invoke();
    }
}
