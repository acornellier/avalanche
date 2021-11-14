using UnityEngine;

public class FallingBlock : GroundableObject
{
    public float speed;

    bool grounded;

    void Update()
    {
        if (grounded)
            return;

        body.velocity = new Vector2(0, -speed);

        if (IsGrounded())
        {
            grounded = true;
            body.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
