using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + speed * Time.deltaTime,
            transform.position.z
        );
    }
}
