using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Player : GroundableObject
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] Collider2D lavaCollider;
    [SerializeField] TMP_Text debugText;

    [Header("Stats")]
    [SerializeField] float walkSpeed = 10;
    [SerializeField] float jumpSpeed = 15;
    [SerializeField] float jumpOffWallSpeed = 10;
    [SerializeField] float slideSpeed = 3;
    [SerializeField] float jumpOffWallBufferTime = 0.3f;
    [SerializeField] float jumpOffWallDuration = 0.2f;
    [SerializeField] float shrinkRate = 50;
    [SerializeField] float expandRate = 3;

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

        var isGrounded = IsGrounded();
        var isCeilinged = IsCeilinged();

        CheckDeath(isGrounded: isGrounded, isCeilinged: isCeilinged);
        if (_isDead) return;

        var newVelocity = body.velocity;

        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var justJumpedOffWall = Time.time - _jumpedOffWallTimestamp < jumpOffWallDuration;
        // reduce horizontal input briefly after jumping off a wall
        // this stops the marshmallow from immediately zooming back to the block
        if (justJumpedOffWall)
            horizontalInput /= 2;

        var wallDirection = GetWallDirection();
        var isHanging =
            !isGrounded
            && !justJumpedOffWall
            && body.velocity.y <= 0
            && (
                (wallDirection < 0 && horizontalInput < 0)
                || (wallDirection > 0 && horizontalInput > 0)
            );

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

#if UNITY_EDITOR
        debugText.text = $@"grounded: {isGrounded}
ceilinged: {isCeilinged}
hanging: {isHanging}
justJumpedOffWall: {justJumpedOffWall}
wallDirection: {wallDirection}
velocity: {body.velocity}";
#endif
    }

    void CheckDeath(bool isGrounded, bool isCeilinged)
    {
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
            var scaleReduction = shrinkRate * Time.deltaTime * new Vector3(0, -1, 0);
            transform.localScale += scaleReduction;

            if (transform.localScale.y >= 0.1 * _startingScale.y)
                return;

            StartCoroutine(Explode());
            return;
        }

        // otherwise, increase scale
        if (transform.localScale.y >= _startingScale.y)
            return;

        var scaleIncrease = expandRate * Time.deltaTime * new Vector3(0, 1, 0);
        transform.localScale += scaleIncrease;

    }

    IEnumerator Explode()
    {
        _isDead = true;
        explosion.Play();
        while (explosion.isPlaying) yield return null;
        OnPlayerDeath?.Invoke();
    }
}
