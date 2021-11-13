using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject fallingBlockPrefab;
    public float secondsBetweenSpawns = 1;
    float nextSpawnTime;
    float nextSpawnHeightOffset;

    public Vector2 spawnSizeMinMax;

    Vector2 screenHalfSizeWorldUnits;

    // Start is called before the first frame update
    void Start()
    {
        screenHalfSizeWorldUnits = new Vector2(
            Camera.main.aspect * Camera.main.orthographicSize,
            Camera.main.orthographicSize
        );
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            nextSpawnHeightOffset += 1;
            nextSpawnTime = Time.time + secondsBetweenSpawns;
            float spawnSize = Random.Range(spawnSizeMinMax.x, spawnSizeMinMax.y);
            var spawnPosition = new Vector2(
                Random.Range(-screenHalfSizeWorldUnits.x+(spawnSize/2), screenHalfSizeWorldUnits.x-(spawnSize/2)),
                screenHalfSizeWorldUnits.y + 0.5f + nextSpawnHeightOffset
            );
            // Quaternion.identity just means zero rotation
            GameObject newBlock = Instantiate(
                fallingBlockPrefab,
                spawnPosition,
                Quaternion.identity
            );
            newBlock.transform.localScale = Vector2.one * spawnSize;
        }
    }
}
