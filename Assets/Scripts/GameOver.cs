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
    [SerializeField] GameObject explosionPrefab;

    bool _gameOver;
    float _maxHeight;

    void Start()
    {
        _maxHeight = 0;
        FindObjectOfType<Player>().OnPlayerDeath += OnGameOver;
        FindObjectOfType<Player>().OnPlayerSquishDeath += OnSquishGameOver;
    }

    void Update()
    {
        if (_gameOver)
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene(0);
        if (player.transform.position.y > _maxHeight)
            _maxHeight = Mathf.Round(player.transform.position.y);
    }

    IEnumerator Explode()
    {
        var explodePosition = new Vector2(
            player.transform.position.x,
            player.transform.position.y
        );

        var newBlock = Instantiate(
            explosionPrefab,
            explodePosition,
            Quaternion.identity
        );
        yield return new WaitForSeconds(1);

        OnGameOver();
    }

    void OnSquishGameOver()
    {
        StartCoroutine(Explode());
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
