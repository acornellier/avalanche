using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : GroundableObject
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] AudioSource oneShotAudioSource;
    [SerializeField] AudioSource statusAudioSource;
    [SerializeField] TMP_Text debugText;

    [SerializeField] Stats stats;
    [SerializeField] Sounds sounds;
    [SerializeField] Sprites sprites;

    [Inject(Id = "Music")] AudioSource _musicAudioSource;

    SpriteRenderer _renderer;

    Vector3 _startingScale;
    Vector3 minScale => stats.stretchMin * _startingScale;
    Vector3 maxScale => stats.stretchMax * _startingScale;

    float _screenHalfWidth;
    float _lastHangingTimestamp;
    int _lastHangingWallDirection;
    float _jumpedOffWallTimestamp;
    float _timeSpentMelting;
    bool _isInLava;
    bool _isDead;

    public event Action OnPlayerDeath;

    protected override void Start()
    {
        base.Start();
        _renderer = GetComponent<SpriteRenderer>();
        _screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        _startingScale = transform.localScale;
    }

    void Update()
    {
        if (_isDead) return;

        var isGrounded = IsGrounded();
        var isCeilinged = IsCeilinged();

        CheckDeath(isGrounded, isCeilinged);
        if (_isDead) return;

        var newVelocity = body.velocity;

        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var justJumpedOffWall = Time.time - _jumpedOffWallTimestamp < stats.jumpOffWallDuration;

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
        {
            _lastHangingTimestamp = Time.time;
            _lastHangingWallDirection = wallDirection;
        }

        var nextToWallAndNotPushingAway = wallDirection * horizontalInput > 0;

        // fixes sticking to walls if left/right is being held
        newVelocity.x = nextToWallAndNotPushingAway && !isGrounded && !justJumpedOffWall
            ? 0
            : Mathf.Lerp(
                body.velocity.x,
                horizontalInput * stats.walkSpeed,
                10 * Time.deltaTime
            );

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded)
            {
                newVelocity.y = stats.jumpSpeed;
                oneShotAudioSource.PlayOneShot(sounds.jump);
            }
            else if (isHanging || Time.time - _lastHangingTimestamp < stats.jumpOffWallBufferTime)
            {
                newVelocity = new Vector2(
                    stats.jumpOffWallSpeed * -_lastHangingWallDirection,
                    stats.jumpSpeed
                );
                _jumpedOffWallTimestamp = Time.time;
                oneShotAudioSource.PlayOneShot(sounds.jump);
            }
        }
        else if (isHanging)
        {
            newVelocity.y = -stats.slideSpeed;
        }

        if (Input.GetKey(KeyCode.Z))
            StretchHorizontally();
        else if (Input.GetKey(KeyCode.X))
            StretchVertically();

        body.velocity = newVelocity;

        if (body.velocity.x < 0 || (isHanging && wallDirection < 0))
            _renderer.sprite = sprites.walkLeft;
        else if (body.velocity.x > 0 || (isHanging && wallDirection > 0))
            _renderer.sprite = sprites.walkRight;
        else
            _renderer.sprite = sprites.idle;

        TeleportAroundEdges();

#if UNITY_EDITOR
        debugText.text = $@"grounded: {isGrounded}
ceilinged: {isCeilinged}
hanging: {isHanging}
justJumpedOffWall: {justJumpedOffWall}
wallDirection: {wallDirection}
velocity: {body.velocity}
scale.y: {transform.localScale.y}";
#endif
    }

    void CheckDeath(bool isGrounded, bool isCeilinged)
    {
        // check for lava death
        if (_isInLava)
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
            StartCoroutine(Explode());
    }

    void StretchHorizontally()
    {
        if (transform.localScale.y <= minScale.y)
            return;

        var change = stats.stretchRate * Time.deltaTime * new Vector3(1, -1, 0);
        transform.localScale += change;
    }

    void StretchVertically()
    {
        if (transform.localScale.y >= maxScale.y)
            return;

        var change = stats.stretchRate * Time.deltaTime * new Vector3(-1, 1, 0);
        transform.localScale += change;
    }

    void TeleportAroundEdges()
    {
        var halfWidth = boxCollider.bounds.size.x / 2 + 1;
        if (transform.position.x + halfWidth < -_screenHalfWidth)
            transform.position = new Vector2(_screenHalfWidth, transform.position.y);
        else if (transform.position.x - halfWidth > _screenHalfWidth)
            transform.position = new Vector2(-_screenHalfWidth, transform.position.y);
    }

    IEnumerator Explode()
    {
        _isDead = true;
        _musicAudioSource.clip = sounds.gameOver;
        _musicAudioSource.Play();
        _renderer.enabled = false;
        explosion.Play();
        while (explosion.isPlaying)
        {
            yield return null;
        }

        OnPlayerDeath?.Invoke();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var lava = col.GetComponent<Lava>();
        if (!lava) return;

        _isInLava = true;
        statusAudioSource.clip = sounds.sizzle;
        statusAudioSource.Play();
    }

    void OnTriggerExit2D(Collider2D col)
    {
        var player = col.GetComponent<Lava>();
        if (!player) return;

        _isInLava = false;
        statusAudioSource.Stop();
    }

    [Serializable]
    class Stats
    {
        public float walkSpeed = 10;
        public float jumpSpeed = 15;
        public float jumpOffWallSpeed = 10;
        public float slideSpeed = 3;
        public float jumpOffWallBufferTime = 0.3f;
        public float jumpOffWallDuration = 0.2f;
        public float stretchRate = 10;
        public float stretchMin = 0.2f;
        public float stretchMax = 1.8f;
    }

    [Serializable]
    class Sounds
    {
        public AudioClip jump;
        public AudioClip sizzle;
        public AudioClip gameOver;
    }

    [Serializable]
    class Sprites
    {
        public Sprite idle;
        public Sprite walkLeft;
        public Sprite walkRight;
    }
}
