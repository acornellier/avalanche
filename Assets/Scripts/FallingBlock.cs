using UnityEngine;

public class FallingBlock : GroundableObject
{
    public float speed;

    bool grounded;

    void Update()
    {
        if (grounded)
            return;

        transform.Translate(speed * Time.deltaTime * Vector3.down);

        if (IsGrounded())
        {
            grounded = true;
            body.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
