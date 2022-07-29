using UnityEngine;
using Zenject;

public class StarSpawner : MonoBehaviour
{
    [SerializeField] GameObject starPrefab;
    [SerializeField] GameObject starHolder;
    [SerializeField] Vector2 spawnSizeMinMax;

    [Inject] Player _player;

    float _distanceBetweenSpawns = 1;
    float _nextSpawnDistance;
    int _nextStarName;

    Vector2 _screenHalfSizeWorldUnits;

    void Start()
    {
        _nextSpawnDistance = 0;
        _screenHalfSizeWorldUnits = new Vector2(
            Camera.main.aspect * Camera.main.orthographicSize,
            Camera.main.orthographicSize
        );
    }

    void Update()
    {
        if (_player == null)
            return;

        if (_player.transform.position.y < _nextSpawnDistance)
            return;

        _nextSpawnDistance += _distanceBetweenSpawns;

        var spawnSize = Random.Range(spawnSizeMinMax.x, spawnSizeMinMax.y);
        switch (_player.transform.position.y)
        {
            case < 30:
                spawnSize /= 4;
                break;
            case > 150:
                _distanceBetweenSpawns = 0.3f;
                break;
            case > 100:
                _distanceBetweenSpawns = 0.4f;
                break;
            case > 50:
                _distanceBetweenSpawns = 0.5f;
                break;
        }

        spawnSize /= 3;

        var spawnPosition = new Vector2(
            Random.Range(
                -_screenHalfSizeWorldUnits.x + spawnSize / 2,
                _screenHalfSizeWorldUnits.x - spawnSize / 2
            ),
            _nextSpawnDistance + 2 * _screenHalfSizeWorldUnits.y
        );

        var newStar = Instantiate(
            starPrefab,
            spawnPosition,
            Quaternion.identity
        );
        newStar.transform.localScale = Vector2.one * spawnSize;
        newStar.name = $"Star {_nextStarName++}";
        newStar.transform.parent = starHolder.transform;
    }
}
