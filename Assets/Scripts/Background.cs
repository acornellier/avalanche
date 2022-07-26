using UnityEngine;

public class Background : MonoBehaviour
{
    public Transform player;
    Renderer rend;
    readonly Color _colorStart = Color.white;
    readonly Color _colorMiddle = Color.blue;
    readonly Color _colorEnd = Color.black;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            player.position.y + 1,
            transform.position.z
        );
        var lerp = .1f;
        switch (player.position.y)
        {
            case < 97:
                lerp = (player.position.y + 3) / 110;
                rend.material.color = Color.Lerp(_colorStart, _colorMiddle, lerp);
                break;
            case < 197:
                lerp = (player.position.y - 97) / 110;
                rend.material.color = Color.Lerp(_colorMiddle, _colorEnd, lerp);
                break;
        }
    }
}
