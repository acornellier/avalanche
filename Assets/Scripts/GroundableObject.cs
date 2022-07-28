using UnityEngine;

public class GroundableObject : MonoBehaviour
{
    [SerializeField] protected LayerMask jumpableMask;

    protected BoxCollider2D boxCollider;
    protected ContactFilter2D contactFilter;
    protected Rigidbody2D body;

    readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[8];
    const float GroundEpsilon = 0.05f;
    const float WallEpsilon = 0.01f;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = jumpableMask;
    }

    protected bool IsGrounded()
    {
        var numHits = boxCollider.Cast(Vector2.down, contactFilter, _hitBuffer, GroundEpsilon);
        for (var hitIndex = 0; hitIndex < numHits; hitIndex++)
        {
            var hit = _hitBuffer[hitIndex];
            if (hit.normal == Vector2.up)
                return hit;
        }

        return false;
    }

    protected bool IsCeilinged()
    {
        var numHits = boxCollider.Cast(Vector2.up, contactFilter, _hitBuffer, GroundEpsilon);
        for (var hitIndex = 0; hitIndex < numHits; hitIndex++)
        {
            var hit = _hitBuffer[hitIndex];
            if (hit.normal == Vector2.down)
                return true;
        }

        return false;
    }

    protected int GetWallDirection()
    {
        var numHits = boxCollider.Cast(Vector2.left, contactFilter, _hitBuffer, WallEpsilon);
        for (var hitIndex = 0; hitIndex < numHits; hitIndex++)
        {
            var hit = _hitBuffer[hitIndex];
            if (hit.normal == Vector2.right)
                return -1;
        }

        numHits = boxCollider.Cast(Vector2.right, contactFilter, _hitBuffer, WallEpsilon);
        for (var hitIndex = 0; hitIndex < numHits; hitIndex++)
        {
            var hit = _hitBuffer[hitIndex];
            if (hit.normal == Vector2.left)
                return 1;
        }

        return 0;
    }
}
