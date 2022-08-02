using UnityEngine;
using Zenject;

[RequireComponent(typeof(Camera))]
public class SmoothCamera : MonoBehaviour
{
    [SerializeField] float smoothSpeed = 5f;
    [SerializeField] Color colorStart = Color.white;
    [SerializeField] Color colorMiddle = Color.blue;
    [SerializeField] Color colorEnd = Color.black;

    [Inject] Player _player;
    [Inject] GameManager _gameManager;

    Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (_gameManager.state != GameState.Playing)
            return;

        UpdatePosition();
        UpdateBackgroundColor();
    }

    void OnValidate()
    {
        GetComponent<Camera>().backgroundColor = colorStart;
    }

    void UpdatePosition()
    {
        var desiredPosition = new Vector3(
            transform.position.x,
            _player.transform.position.y + 1,
            transform.position.z
        );

        var smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed / 2 * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }

    void UpdateBackgroundColor()
    {
        float lerp;
        switch (_player.transform.position.y)
        {
            case < 97:
                lerp = (_player.transform.position.y + 3) / 110;
                _camera.backgroundColor = Color.Lerp(colorStart, colorMiddle, lerp);
                break;
            case < 197:
                lerp = (_player.transform.position.y - 97) / 110;
                _camera.backgroundColor = Color.Lerp(colorMiddle, colorEnd, lerp);
                break;
        }
    }
}
