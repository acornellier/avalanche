using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] Transform player;

    readonly Color _colorStart = Color.white;
    readonly Color _colorMiddle = Color.blue;
    readonly Color _colorEnd = Color.black;

    Renderer _rend;

    void Start()
    {
        _rend = GetComponent<Renderer>();
    }

    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            player.position.y + 1,
            transform.position.z
        );

        float lerp;
        switch (player.position.y)
        {
            case < 97:
                lerp = (player.position.y + 3) / 110;
                _rend.material.color = Color.Lerp(_colorStart, _colorMiddle, lerp);
                break;
            case < 197:
                lerp = (player.position.y - 97) / 110;
                _rend.material.color = Color.Lerp(_colorMiddle, _colorEnd, lerp);
                break;
        }
    }
}
