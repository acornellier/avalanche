using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject gameBackground;
    public GameObject starHolder;
    public GameObject lava;
    public GameObject playerObject;
    public Text score;
    bool gameOver;
    public Transform player;
    public GameObject camera;
    float maxHeight;
    public GameObject explosionPrefab;

    void Start()
    {
        maxHeight = 0;
        FindObjectOfType<Player>().OnPlayerDeath += OnGameOver;
        FindObjectOfType<Player>().OnPlayerSquishDeath += OnSquishGameOver;
    }

    void Update()
    {
        if (gameOver)
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene(0);
        if (player.transform.position.y > maxHeight)
            maxHeight = Mathf.Round(player.transform.position.y);
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
        score.text = $"{maxHeight}ft";
        gameOver = true;
        camera.transform.position = new Vector3(21.76f, 18.11f, -12.13f);
        Camera.main.orthographicSize = 20;
    }
}
