using UnityEngine;

public class GroundableObject : MonoBehaviour
{
    [SerializeField]
    protected LayerMask jumpableMask;

    protected BoxCollider2D boxCollider;
    protected Rigidbody2D body;
    protected ContactFilter2D contactFilter;

    readonly float groundEpsilon = 0.05f;
    readonly RaycastHit2D[] hitBuffer = new RaycastHit2D[8];

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
    }

    protected bool IsGrounded()
    {
        int numHits = boxCollider.Cast(Vector2.down, hitBuffer, groundEpsilon);
        for (int hitIndex = 0; hitIndex < numHits; hitIndex++)
        {
            var hit = hitBuffer[hitIndex];
            if (hit.normal == Vector2.up)
                return true;
        }

        return false;
    }

    protected bool IsCeilinged()
    {
        int numHits = boxCollider.Cast(Vector2.up, hitBuffer, groundEpsilon);
        for (int hitIndex = 0; hitIndex < numHits; hitIndex++)
        {
            var hit = hitBuffer[hitIndex];
            if (hit.normal == Vector2.down)
                return true;
        }

        return false;
    }
}
