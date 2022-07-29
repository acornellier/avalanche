using UnityEngine;
using Zenject;

public class MyCamera : MonoBehaviour
{
    [SerializeField] Transform player;

    [Inject] GameManager _gameManager;

    void Update()
    {
        if (_gameManager.state != GameState.Playing)
            return;

        transform.position = new Vector3(
            transform.position.x,
            player.position.y + 1,
            transform.position.z
        );
    }
}
