using System.Linq;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] GameObject fallingBlockPrefab;
    [SerializeField] GameObject blockHolder;
    [SerializeField] float secondsBetweenSpawns = 1;
    [SerializeField] Vector2 spawnSizeMinMax;

    Player _player;
    Vector2 _screenHalfSizeWorldUnits;

    float _nextSpawnTime;
    int _nextBlockName;

    void Start()
    {
        _player = FindObjectOfType<Player>();
        _screenHalfSizeWorldUnits = new Vector2(
            Camera.main.aspect * Camera.main.orthographicSize,
            Camera.main.orthographicSize
        );
    }

    void Update()
    {
        if (Time.time < _nextSpawnTime)
            return;

        _nextSpawnTime = Time.time + secondsBetweenSpawns;

        var newBlock = SpawnNonCollidingBlock();
        if (newBlock != null)
            newBlock.name = $"Block {_nextBlockName++}";
    }

    GameObject SpawnNonCollidingBlock()
    {
        var spawnSize = Random.Range(spawnSizeMinMax.x, spawnSizeMinMax.y);
        var spawnX = Random.Range(
            -_screenHalfSizeWorldUnits.x + spawnSize / 2,
            _screenHalfSizeWorldUnits.x - spawnSize / 2
        );
        var spawnY = 2 * _screenHalfSizeWorldUnits.y;
        if (_player)
            spawnY += _player.transform.position.y;

        for (var i = 0; i < 10; i++)
        {
            var newBlock = Instantiate(
                fallingBlockPrefab,
                new Vector2(spawnX, spawnY),
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
            if (!isColliding) return newBlock;

            Destroy(newBlock);
            spawnY += 5;
        }

        Debug.LogError("Failed to find a position to spawn a block after 10 attempts");
        return null;
    }
}
