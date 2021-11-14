using UnityEngine;

public class FallingBlock : GroundableObject
{
    public float speed;

    void Update()
    {
        if (!IsGrounded())
        {
            body.velocity = new Vector2(0, -speed);
        }
    }
}
