using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public Transform player;
    Renderer rend;
    Color colorStart = Color.white;
    Color colorMiddle = Color.blue;
    Color colorEnd = Color.black;
    float duration = 1.0f;

    void Start()
    {
        rend = GetComponent<Renderer> ();
    }

    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            player.position.y + 1,
            transform.position.z
        );
        float lerp = .1f;
        if (player.position.y < 97) {
            lerp = (player.position.y + 3) / 110;
            rend.material.color = Color.Lerp(colorStart, colorMiddle, lerp);
        }
        else if (player.position.y < 197) {
            lerp = (player.position.y - 97) / 110;
            rend.material.color = Color.Lerp(colorMiddle, colorEnd, lerp);
        }
        
    }
}
