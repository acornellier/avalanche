using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Player : GroundableObject
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] AudioSource oneShotAudioSource;
    [SerializeField] AudioSource statusAudioSource;
    [SerializeField] TMP_Text debugText;

    [SerializeField] Stats stats;
    [SerializeField] Sounds sounds;

    Renderer _renderer;

    Vector3 _startingScale;
    float _screenHalfWidth;
    float _lastHangingTimestamp;
    int _lastHangingWallDirection;
    float _jumpedOffWallTimestamp;
    float _timeSpentMelting;
    bool _isMelting;
    bool _isDead;

    public event Action OnPlayerDeath;

    protected override void Start()
    {
        base.Start();
        _renderer = GetComponent<Renderer>();
        oneShotAudioSource = GetComponent<AudioSource>();
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
        if (nextToWallAndNotPushingAway && !isGrounded && !justJumpedOffWall)
            newVelocity.x = 0;
        else
            newVelocity.x = Mathf.Lerp(body.velocity.x, horizontalInput * stats.walkSpeed, 0.05f);

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
        if (_isMelting)
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
            var scaleReduction = stats.shrinkRate * Time.deltaTime * new Vector3(0, -1, 0);
            transform.localScale += scaleReduction;

            if (transform.localScale.y >= 0.1 * _startingScale.y)
                return;

            StartCoroutine(Explode());
            return;
        }

        // otherwise, increase scale
        if (transform.localScale.y >= _startingScale.y)
            return;

        var scaleIncrease = stats.expandRate * Time.deltaTime * new Vector3(0, 1, 0);
        transform.localScale += scaleIncrease;
    }

    IEnumerator Explode()
    {
        _isDead = true;
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

        _isMelting = true;
        statusAudioSource.clip = sounds.sizzle;
        statusAudioSource.Play();
    }

    void OnTriggerExit2D(Collider2D col)
    {
        var player = col.GetComponent<Lava>();
        if (!player) return;

        _isMelting = false;
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
        public float shrinkRate = 50;
        public float expandRate = 3;
    }

    [Serializable]
    class Sounds
    {
        public AudioClip jump;
        public AudioClip sizzle;
    }
}
