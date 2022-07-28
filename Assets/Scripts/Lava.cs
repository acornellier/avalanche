using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] AudioSource sizzle;

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.up);
    }
}
