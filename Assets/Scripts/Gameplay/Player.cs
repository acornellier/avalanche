using System;
using System.Collections;
using Animancer;
using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SpriteRenderer), typeof(AnimancerComponent))]
public class Player : GroundableObject
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] ParticleSystem burning;
    [SerializeField] AudioSource oneShotAudioSource;
    [SerializeField] AudioSource statusAudioSource;
    [SerializeField] TMP_Text debugText;

    [SerializeField] Stats stats;
    [SerializeField] AnimationClips clips;
    [SerializeField] Sounds sounds;

    [Inject(Id = "Music")] AudioSource _musicAudioSource;

    SpriteRenderer _renderer;
    AnimancerComponent _animancer;

    Vector3 _startingScale;
    Vector3 minScale => stats.stretchMin * _startingScale;
    Vector3 maxScale => stats.stretchMax * _startingScale;

    State _state = State.None;
    float _screenHalfWidth;
    float _lastHangingTimestamp;
    int _lastHangingWallDirection;
    float _jumpedOffWallTimestamp;
    float _timeSpentMelting;
    bool _isInLava;
    bool _isDead;
    bool _isGrounded;
    bool _isCeilinged;
    int _wallDirection;

    public event Action OnPlayerDeath;

    protected void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animancer = GetComponent<AnimancerComponent>();
        _screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        _startingScale = transform.localScale;
    }

    void Update()
    {
        if (_isDead) return;

        _isGrounded = IsGrounded();
        _isCeilinged = IsCeilinged();
        CheckDeath();
        if (_isDead) return;

        _wallDirection = GetWallDirection();
        var horizontalInput = Input.GetAxisRaw("Horizontal");

        UpdateVelocity(horizontalInput);
        UpdateScale(horizontalInput);
        UpdateAnimator();

        TeleportAroundEdges();

#if UNITY_EDITOR
        UpdateDebugText();
#endif
    }

    void UpdateAnimator()
    {
        if (_state == State.Airborne && _isGrounded)
            _state = State.None;

        if (_state != State.None) return;

        if (_isInLava)
            _animancer.Play(clips.sad);
        else if (Mathf.Abs(body.velocity.x) > 0.1)
            _animancer.Play(clips.walk);
        else
            _animancer.Play(clips.idle);
    }

    void UpdateVelocity(float horizontalInput)
    {
        var newVelocity = body.velocity;

        var isHanging = IsHanging(horizontalInput);

        if (isHanging)
        {
            _lastHangingTimestamp = Time.time;
            _lastHangingWallDirection = _wallDirection;
        }

        var nextToWallAndNotPushingAway = _wallDirection * horizontalInput > 0;

        // fixes sticking to walls if left/right is being held
        newVelocity.x = nextToWallAndNotPushingAway && !_isGrounded && !JustJumpedOffWall()
            ? 0
            : Mathf.Lerp(
                body.velocity.x,
                horizontalInput * stats.walkSpeed,
                10 * Time.deltaTime
            );

        if (Input.GetKeyDown(KeyCode.UpArrow))
            newVelocity = TryJump(_isGrounded, isHanging, newVelocity);
        else if (isHanging)
            newVelocity.y = -stats.slideSpeed;

        body.velocity = newVelocity;
    }

    bool IsHanging(float horizontalInput)
    {
        return !_isGrounded
               && !JustJumpedOffWall()
               && body.velocity.y <= 0
               && (
                   (_wallDirection < 0 && horizontalInput < 0)
                   || (_wallDirection > 0 && horizontalInput > 0)
               );
    }

    bool JustJumpedOffWall()
    {
        return Time.time - _jumpedOffWallTimestamp < stats.jumpOffWallDuration;
    }

    void UpdateScale(float horizontalInput)
    {
        if (Input.GetKey(KeyCode.Z))
            StretchHorizontally();
        else if (Input.GetKey(KeyCode.X))
            StretchVertically();

        // facing direction
        if ((horizontalInput < 0 && transform.localScale.x > 0) ||
            (horizontalInput > 0 && transform.localScale.x < 0))
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }

    Vector2 TryJump(bool isGrounded, bool isHanging, Vector2 newVelocity)
    {
        var jumping = false;
        if (isGrounded)
        {
            jumping = true;
            newVelocity.y = stats.jumpSpeed;
        }
        else if (isHanging || Time.time - _lastHangingTimestamp < stats.jumpOffWallBufferTime)
        {
            jumping = true;
            newVelocity = new Vector2(
                stats.jumpOffWallSpeed * -_lastHangingWallDirection,
                stats.jumpSpeed
            );
            _jumpedOffWallTimestamp = Time.time;
        }

        if (jumping)
        {
            oneShotAudioSource.PlayOneShot(sounds.jump);
            _state = State.Jumping;
            var animancerState = _animancer.Play(clips.jump);
            animancerState.Events.OnEnd = () =>
            {
                _state = State.Airborne;
                _animancer.Play(clips.airborne);
            };
        }

        return newVelocity;
    }

#if UNITY_EDITOR
    void UpdateDebugText()
    {
        var justJumpedOffWall = JustJumpedOffWall();
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var isHanging = IsHanging(horizontalInput);

        debugText.text = $@"grounded: {_isGrounded}
ceilinged: {IsCeilinged()}
hanging: {isHanging}
justJumpedOffWall: {justJumpedOffWall}
wallDirection: {_wallDirection}
velocity: {body.velocity}";
    }
#endif

    void CheckDeath()
    {
        // check for squishing death
        if (_isGrounded && _isCeilinged)
        {
            Die();
            StartCoroutine(Explode());
            return;
        }

        // check for lava death
        if (_isInLava)
        {
            if (_timeSpentMelting > 1)
            {
                Die();
                Melt();
                return;
            }

            _timeSpentMelting += Time.deltaTime;
            _renderer.material.color = Color.Lerp(Color.white, Color.black, _timeSpentMelting);
        }
    }

    void StretchHorizontally()
    {
        if (transform.localScale.y <= minScale.y)
            return;

        var signOfX = Mathf.Sign(transform.localScale.x);
        var change = stats.stretchRate * Time.deltaTime * new Vector3(1 * signOfX, -1, 0);
        transform.localScale += change;
    }

    void StretchVertically()
    {
        if (transform.localScale.y >= maxScale.y)
            return;

        var signOfX = Mathf.Sign(transform.localScale.x);
        var change = stats.stretchRate * Time.deltaTime * new Vector3(-1 * signOfX, 1, 0);
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

    void Die()
    {
        _isDead = true;
        _musicAudioSource.clip = sounds.gameOver;
        _musicAudioSource.Play();
    }

    IEnumerator Explode()
    {
        _renderer.enabled = false;
        explosion.Play();
        while (explosion.isPlaying)
        {
            yield return null;
        }

        OnPlayerDeath?.Invoke();
    }

    void Melt()
    {
        _state = State.Melting;
        var animancerState = _animancer.Play(clips.melt);
        animancerState.Events.OnEnd = () => OnPlayerDeath?.Invoke();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var lava = col.GetComponent<Lava>();
        if (!lava) return;

        _isInLava = true;
        burning.Play();
        statusAudioSource.clip = sounds.sizzle;
        statusAudioSource.Play();
    }

    void OnTriggerExit2D(Collider2D col)
    {
        var lava = col.GetComponent<Lava>();
        if (!lava) return;

        _isInLava = false;
        burning.Stop();
        statusAudioSource.Stop();
    }

    enum State
    {
        None,
        Jumping,
        Airborne,
        Melting,
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
    struct AnimationClips
    {
        public AnimationClip idle;
        public AnimationClip walk;
        public AnimationClip jump;
        public AnimationClip airborne;
        public AnimationClip sad;
        public AnimationClip melt;
    }
}
