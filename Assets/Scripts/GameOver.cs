using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject gameBackground;
    [SerializeField] GameObject starHolder;
    [SerializeField] GameObject lava;
    [SerializeField] Text score;
    [SerializeField] Transform player;

    bool _gameOver;
    float _maxHeight;

    void Start()
    {
        _maxHeight = 0;
        FindObjectOfType<Player>().OnPlayerDeath += OnGameOver;
    }

    void Update()
    {
        if (_gameOver)
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene(0);
        if (player.transform.position.y > _maxHeight)
            _maxHeight = Mathf.Round(player.transform.position.y);
    }

    void OnGameOver()
    {
        gameOverScreen.SetActive(true);
        gameBackground.SetActive(false);
        starHolder.SetActive(false);
        lava.SetActive(false);
        score.text = $"{_maxHeight}ft";
        _gameOver = true;
        Camera.main.transform.position = new Vector3(21.76f, 18.11f, -12.13f);
        Camera.main.orthographicSize = 20;
    }
}
