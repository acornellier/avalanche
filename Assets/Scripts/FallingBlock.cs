using UnityEngine;

public class FallingBlock : GroundableObject
{
    public float speed;

    bool _grounded;

    void Update()
    {
        if (_grounded)
            return;

        transform.Translate(0, -speed * Time.deltaTime, 0);

         if (!IsGrounded())
            return;

        _grounded = true;
    }
}
