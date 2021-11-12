using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            player.position.y + 1,
            transform.position.z
        );
    }
}
