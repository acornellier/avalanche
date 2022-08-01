using UnityEngine;
using Zenject;

[RequireComponent(typeof(BoxCollider2D))]
public class Lava : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float minDistanceToSpeedUp = 5f;
    [SerializeField] float distanceSpeedMultiplier = 0.1f;

    [Inject] Player _player;

    BoxCollider2D _collider;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        var distanceFromPlayer = Physics2D.Distance(_collider, _player.boxCollider).distance;
        var effectiveDistance = Mathf.Max(distanceFromPlayer - minDistanceToSpeedUp, 0);
        var adjustedSpeed = speed + distanceSpeedMultiplier * effectiveDistance;

        transform.Translate(adjustedSpeed * Time.deltaTime * Vector3.up);
    }
}
