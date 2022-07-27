using UnityEngine;

public class FallingBlock : GroundableObject
{
    [SerializeField] float speed;

    bool _grounded;

    protected override void Start()
    {
        base.Start();
        SetRandomColor();
    }

    void Update()
    {
        if (_grounded)
            return;

        // body.velocity = new Vector2(0, -speed);
        transform.Translate(0, -speed * Time.deltaTime, 0);

         if (!IsGrounded())
            return;

        _grounded = true;
    }

    void SetRandomColor()
    {
        GetComponent<Renderer>().material.color = UnityEngine.Random.Range(0, 6) switch
        {
            0 => Color.yellow,
            1 => Color.blue,
            2 => Color.magenta,
            3 => Color.red,
            4 => Color.cyan,
            _ => Color.green
        };
    }
}
