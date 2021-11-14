using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Player player;
    public GameObject fallingBlockPrefab;
    public float secondsBetweenSpawns = 1;

    float nextSpawnTime;
    int nextBlockName;
    bool gameOver;

    public Vector2 spawnSizeMinMax;

    Vector2 screenHalfSizeWorldUnits;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnPlayerDeath += OnGameOver;
        screenHalfSizeWorldUnits = new Vector2(
            Camera.main.aspect * Camera.main.orthographicSize,
            Camera.main.orthographicSize
        );
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver || Time.time < nextSpawnTime)
            return;

        nextSpawnTime = Time.time + secondsBetweenSpawns;
        float spawnSize = Random.Range(spawnSizeMinMax.x, spawnSizeMinMax.y);
        var spawnPosition = new Vector2(
            Random.Range(
                -screenHalfSizeWorldUnits.x + (spawnSize / 2),
                screenHalfSizeWorldUnits.x - (spawnSize / 2)
            ),
            player.transform.position.y + 2 * screenHalfSizeWorldUnits.y
        );
        // Quaternion.identity just means zero rotation
        GameObject newBlock = Instantiate(fallingBlockPrefab, spawnPosition, Quaternion.identity);
        newBlock.name = $"Block {nextBlockName++}";
        newBlock.transform.localScale = Vector2.one * spawnSize;
    }

    void OnGameOver()
    {
        gameOver = true;
    }
}
