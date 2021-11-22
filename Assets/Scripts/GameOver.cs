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
        gameBackground.SetActive (false);
        starHolder.SetActive (false);
        lava.SetActive (false);
        score.text = $"{Mathf.Round(player.position.y)}ft";
        gameOver = true;
        camera.transform.position = new Vector3(12.2f, 14.61f, -12.13f);
        Camera.main.orthographicSize = 20;
 
        Destroy (playerObject);
    }
}
