using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public Player player;
    public GameObject starPrefab;
    public GameObject starHolder;
    public float distanceBetweenSpawns = 1;

    float nextSpawnDistance;
    int nextStarName;

    public Vector2 spawnSizeMinMax;

    Vector2 screenHalfSizeWorldUnits;

    // Start is called before the first frame update
    void Start()
    {
        nextSpawnDistance = 0;
        player = FindObjectOfType<Player>();
        screenHalfSizeWorldUnits = new Vector2(
            Camera.main.aspect * Camera.main.orthographicSize,
            Camera.main.orthographicSize
        );
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y < nextSpawnDistance)
            return;
        nextSpawnDistance = nextSpawnDistance + distanceBetweenSpawns;
        var spawnSize = Random.Range(spawnSizeMinMax.x, spawnSizeMinMax.y);
        if (player.transform.position.y < 30) {
            spawnSize = spawnSize / 4;
        }
 
        if (player.transform.position.y > 50) {
            distanceBetweenSpawns = 0.5f;
        }
        if (player.transform.position.y > 100) {
            distanceBetweenSpawns = 0.4f;
        }
        if (player.transform.position.y > 150) {
            distanceBetweenSpawns = 0.3f;
        }
        spawnSize = spawnSize / 3;

        var spawnPosition = new Vector2(
            Random.Range(
                -screenHalfSizeWorldUnits.x + (spawnSize / 2),
                screenHalfSizeWorldUnits.x - (spawnSize / 2)
            ),
            nextSpawnDistance + 2 * screenHalfSizeWorldUnits.y
        );

        var newStar = Instantiate(
            starPrefab,
            spawnPosition,
            Quaternion.identity
        );
        newStar.transform.localScale = Vector2.one * spawnSize;
        newStar.name = $"Star {nextStarName++}";
        newStar.transform.parent = starHolder.transform;
    }
}
