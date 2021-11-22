using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public GameObject explosionPrefab;

    void Start() {
        FindObjectOfType<Player>().OnPlayerDeath += OnGameOver;
    }

    void Update()
    {
        if (gameOver) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene(0);
            }
        }    
    }

    void OnGameOver() {
        float positionX = player.position.x;
        float positionY = player.position.y;
        var explodePosition = new Vector2(
            positionX,
            positionY - 0.5f
        );

        StartCoroutine(ZoomOutCamera(Mathf.Round(player.position.y)));
        // Destroy (playerObject);

        GameObject newBlock = Instantiate(
            explosionPrefab,
            explodePosition,
            Quaternion.identity
        );
        
    }

    IEnumerator ZoomOutCamera(float result) {
        yield return new WaitForSeconds(1);
        gameOverScreen.SetActive (true);
        gameBackground.SetActive (false);
        starHolder.SetActive (false);
        lava.SetActive (false);
        score.text = $"{result}ft";
        gameOver = true;
        camera.transform.position = new Vector3(12.2f, 14.61f, -12.13f);
        Camera.main.orthographicSize = 20;
    }
}
