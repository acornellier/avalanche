using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public Player player;
    public GameObject fallingBlockPrefab;
    public GameObject blockHolder;
    public float secondsBetweenSpawns = 1;

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
        print(_gameOver);
        if (_gameOver || Time.time < _nextSpawnTime)
            return;

        _nextSpawnTime = Time.time + secondsBetweenSpawns;
        var spawnSize = Random.Range(spawnSizeMinMax.x, spawnSizeMinMax.y);

        var newBlock = SpawnNonCollidingBlock(spawnSize);
        newBlock.name = $"Block {_nextBlockName++}";
        newBlock.GetComponent<Renderer>().material.color = Random.Range(0, 6) switch
        {
            0 => Color.yellow,
            1 => Color.blue,
            2 => Color.magenta,
            3 => Color.red,
            4 => Color.cyan,
            _ => Color.green
        };
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
            var hits = Physics2D.OverlapBoxAll(newCollider.bounds.center, newCollider.bounds.size, 0);

            var isColliding = hits.Any(hit => hit != newCollider);

            if (isColliding)
                Destroy(newBlock);
            else
                return newBlock;
        }

        throw new Exception("Failed to find a position to spawn a block after 10 attempts");
    }

    void OnGameOver()
    {
        _gameOver = true;
    }
}
