using UnityEngine;

public class GroundableObject : MonoBehaviour
{
    [SerializeField]
    protected LayerMask jumpableMask;

    protected BoxCollider2D boxCollider;
    protected Rigidbody2D body;
    protected ContactFilter2D contactFilter;

    readonly RaycastHit2D[] hitBuffer = new RaycastHit2D[8];
    readonly float groundEpsilon = 0.05f;
    readonly float wallEpsilon = 0.01f;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = jumpableMask;
    }

    protected bool IsGrounded()
    {
        int numHits = boxCollider.Cast(Vector2.down, contactFilter, hitBuffer, groundEpsilon);
        for (int hitIndex = 0; hitIndex < numHits; hitIndex++)
        {
            var hit = hitBuffer[hitIndex];
            if (hit.normal == Vector2.up)
                return hit;
        }

        return false;
    }

    protected bool IsCeilinged()
    {
        int numHits = boxCollider.Cast(Vector2.up, contactFilter, hitBuffer, groundEpsilon);
        for (int hitIndex = 0; hitIndex < numHits; hitIndex++)
        {
            var hit = hitBuffer[hitIndex];
            if (hit.normal == Vector2.down)
                return true;
        }

        return false;
    }

    protected float GetWallDirection()
    {
        int numHits = boxCollider.Cast(Vector2.left, contactFilter, hitBuffer, wallEpsilon);
        for (int hitIndex = 0; hitIndex < numHits; hitIndex++)
        {
            var hit = hitBuffer[hitIndex];
            if (hit.normal == Vector2.right)
                return -1;
        }

        numHits = boxCollider.Cast(Vector2.right, contactFilter, hitBuffer, wallEpsilon);
        for (int hitIndex = 0; hitIndex < numHits; hitIndex++)
        {
            var hit = hitBuffer[hitIndex];
            if (hit.normal == Vector2.left)
                return 1;
        }

        return 0;

        var leftBoxCastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.left,
            wallEpsilon,
            jumpableMask
        );

        if (leftBoxCastHit.collider != null)
            return -1;

        var rightBoxCastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.right,
            wallEpsilon,
            jumpableMask
        );

        if (rightBoxCastHit.collider != null)
            return 1;

        return 0;
    }
}
