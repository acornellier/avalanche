using UnityEngine;

public class GroundableObject : MonoBehaviour
{
    [SerializeField]
    protected LayerMask jumpableMask;

    protected BoxCollider2D boxCollider;
    protected Rigidbody2D body;
    protected ContactFilter2D contactFilter;

    readonly float groundEpsilon = 0.05f;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
    }

    protected bool IsGrounded()
    {
        var boxCastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            groundEpsilon,
            jumpableMask
        );

        return boxCastHit.collider != null && boxCastHit.collider.gameObject != gameObject;
    }
}
