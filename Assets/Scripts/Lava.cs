using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] float speed;

    void Update()
    {
        transform.Translate( speed * Time.deltaTime * Vector2.up);
    }
}
