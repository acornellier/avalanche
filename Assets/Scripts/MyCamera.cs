using UnityEngine;

public class MyCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] bool gameOver;

    void Start()
    {
        FindObjectOfType<Player>().OnPlayerDeath += OnGameOver;
        FindObjectOfType<Player>().OnPlayerSquishDeath += OnGameOver;
    }

    void Update()
    {
        if (!gameOver)
            transform.position = new Vector3(
                transform.position.x,
                player.position.y + 1,
                transform.position.z
            );
    }

    void OnGameOver()
    {
        gameOver = true;
    }
}
