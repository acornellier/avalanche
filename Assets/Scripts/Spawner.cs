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
        var spawnSize = Random.Range(spawnSizeMinMax.x, spawnSizeMinMax.y);

        var newBlock = SpawnNonCollidingBlock(spawnSize);
        newBlock.name = $"Block {nextBlockName++}";
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
