using UnityEngine;

public class FallingBlock : GroundableObject
{
    [SerializeField] float speed;

    int _timesGrounded;

    // only freeze after several times grounded
    // in case block lands on another block that's still falling
    const int MaxTimesGrounded = 5;

    protected override void Start()
    {
        base.Start();
        SetRandomColor();
    }

    void FixedUpdate()
    {
        if (_timesGrounded >= MaxTimesGrounded)
            return;

        if (IsGrounded())
        {
            _timesGrounded += 1;
            body.velocity = Vector2.zero;

            if (_timesGrounded >= MaxTimesGrounded)
                body.constraints = RigidbodyConstraints2D.FreezeAll;

            return;
        }

        body.velocity = new Vector2(0, -speed);
    }

    void SetRandomColor()
    {
        GetComponent<Renderer>().material.color = Random.Range(0, 6) switch
        {
            0 => Color.yellow,
            1 => Color.blue,
            2 => Color.magenta,
            3 => Color.red,
            4 => Color.cyan,
            _ => Color.green,
        };
    }
}
