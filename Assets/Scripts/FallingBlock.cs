using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    float speed = 7;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }
}
