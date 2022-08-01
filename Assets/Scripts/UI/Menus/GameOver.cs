using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject gameOverUi;
    [SerializeField] TMP_Text score;

    [Inject] Player _player;
    [Inject] Lava _lava;
    [Inject] BlockSpawner _blockSpawner;
    [Inject] HeightTracker _heightTracker;
    [Inject] GameManager _gameManager;

    void OnEnable()
    {
        _player.OnPlayerDeath += OnGameOver;
    }

    void OnDisable()
    {
        _player.OnPlayerDeath -= OnGameOver;
    }

    void Update()
    {
        if (_gameManager.state == GameState.GameOver && Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene("Game");
    }

    void OnGameOver()
    {
        gameOverUi.SetActive(true);
        _lava.gameObject.SetActive(false);
        _blockSpawner.gameObject.SetActive(false);

        score.text = $"{_heightTracker.maxHeight}ft";

        Camera.main.transform.position = new Vector3(21.76f, 18.11f, -12.13f);
        Camera.main.orthographicSize = 20;
    }
}
