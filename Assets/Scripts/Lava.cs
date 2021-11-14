using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float positionFactor = 0.000000001f;
    int frameCount = 0;

    void Update()
    {
        frameCount += 1;
        transform.position = new Vector3 (transform.position.x, Time.timeSinceLevelLoad -90, transform.position.z);
    }
}
