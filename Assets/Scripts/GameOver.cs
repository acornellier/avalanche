using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverScreen;
    public Text score;
    bool gameOver;
    public Transform player;
    public GameObject camera;

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
        gameOverScreen.SetActive (true);
        score.text = player.position.y.ToString();
        gameOver = true;
        camera.transform.position = new Vector3(28.5f, 14.61f, -12.13f);
        Camera.main.orthographicSize = 20;
    }
}
