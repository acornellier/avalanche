using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public Player player;
    public GameObject fallingBlockPrefab;
    public GameObject blockHolder;
    public float secondsBetweenSpawns = 1;

    float nextSpawnTime;
    int nextBlockName;
    bool gameOver;

    public Vector2 spawnSizeMinMax;

    Vector2 screenHalfSizeWorldUnits;

    public Color cyan = new Color(0, 1, 1, 1);
    public Color red = new Color(1, 0, 0, 1);
    public Color magenta = new Color(1, 0, 1, 1);
    public Color green = new Color(0, 1, 0, 1);
    public Color blue = new Color(0, 0, 1, 1);
    public Color clear = new Color(0, 0, 0, 0);
    // public Color yellow = new Color(1, 0.92, 0.016, 1);
    public List<Color> colors = new List<Color>
        { 
            new Color(0, 1, 1, 1),
            new Color(1, 0, 0, 1),
            new Color(1, 0, 1, 1),
            new Color(0, 1, 0, 1),
            new Color(0, 0, 1, 1),
            new Color(0, 0, 0, 0),
        };

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
        var spawnSize = Random.Range(spawnSizeMinMax.x, spawnSizeMinMax.y);

        var newBlock = SpawnNonCollidingBlock(spawnSize);
        newBlock.name = $"Block {nextBlockName++}";
        int index = Random.Range(0, colors.Count);
        newBlock.GetComponent<Renderer>().material.color = colors[index];
    }

    GameObject SpawnNonCollidingBlock(float spawnSize)
    {
        for (int i = 0; i < 10; i++)
        {
            var spawnPosition = new Vector2(
                Random.Range(
                    -screenHalfSizeWorldUnits.x + (spawnSize / 2),
                    screenHalfSizeWorldUnits.x - (spawnSize / 2)
                ),
                player.transform.position.y + 2 * screenHalfSizeWorldUnits.y
            );

            GameObject newBlock = Instantiate(
                fallingBlockPrefab,
                spawnPosition,
                Quaternion.identity
            );

            newBlock.transform.localScale = Vector2.one * spawnSize;
            newBlock.transform.parent = blockHolder.transform;

            var collider = newBlock.GetComponent<BoxCollider2D>();
            var hits = Physics2D.OverlapBoxAll(collider.bounds.center, collider.bounds.size, 0);

            bool isColliding = false;
            foreach (var hit in hits)
            {
                if (hit != collider)
                {
                    isColliding = true;
                    break;
                }
            }

            if (isColliding)
                Destroy(newBlock);
            else
                return newBlock;
        }

        throw new System.Exception("Failed to find a position to spawn a block after 10 attempts");
    }

    void OnGameOver()
    {
        gameOver = true;
    }
}
