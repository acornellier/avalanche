using System.Linq;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject fallingBlockPrefab;
    [SerializeField] GameObject blockHolder;
    [SerializeField] float secondsBetweenSpawns = 1;

    float _nextSpawnTime;
    int _nextBlockName;
    bool _gameOver;

    public Vector2 spawnSizeMinMax;

    Vector2 _screenHalfSizeWorldUnits;

    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnPlayerDeath += OnGameOver;
        _screenHalfSizeWorldUnits = new Vector2(
            Camera.main.aspect * Camera.main.orthographicSize,
            Camera.main.orthographicSize
        );
    }

    void Update()
    {
        if (_gameOver || Time.time < _nextSpawnTime)
            return;

        _nextSpawnTime = Time.time + secondsBetweenSpawns;
        var spawnSize = Random.Range(spawnSizeMinMax.x, spawnSizeMinMax.y);

        var newBlock = SpawnNonCollidingBlock(spawnSize);
        if (newBlock == null)
            return;

        newBlock.name = $"Block {_nextBlockName++}";
    }

    GameObject SpawnNonCollidingBlock(float spawnSize)
    {
        for (var i = 0; i < 10; i++)
        {
            var spawnPosition = new Vector2(
                Random.Range(
                    -_screenHalfSizeWorldUnits.x + spawnSize / 2,
                    _screenHalfSizeWorldUnits.x - spawnSize / 2
                ),
                player.transform.position.y + 2 * _screenHalfSizeWorldUnits.y
            );

            var newBlock = Instantiate(
                fallingBlockPrefab,
                spawnPosition,
                Quaternion.identity
            );

            newBlock.transform.localScale = Vector2.one * spawnSize;
            newBlock.transform.parent = blockHolder.transform;

            var newCollider = newBlock.GetComponent<BoxCollider2D>();
            var hits = Physics2D.OverlapBoxAll(
                newCollider.bounds.center,
                newCollider.bounds.size,
                0
            );

            var isColliding = hits.Any(hit => hit != newCollider);

            if (isColliding)
                Destroy(newBlock);
            else
                return newBlock;
        }

        Debug.LogError("Failed to find a position to spawn a block after 10 attempts");
        return null;
    }

    void OnGameOver()
    {
        _gameOver = true;
    }
}
