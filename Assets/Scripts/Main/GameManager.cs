using System;
using UnityEngine;
using Zenject;

public enum GameState
{
    Playing,
    Paused,
    GameOver,
}

public class GameManager : IInitializable, ITickable
{
    public GameState state { get; private set; } = GameState.Playing;

    [Inject] Player _player;

    public event Action<bool> OnGamePauseChange;

    public void Initialize()
    {
        _player.OnPlayerDeath += OnPlayerDeath;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        switch (state)
        {
            case GameState.Playing:
                state = GameState.Paused;
                Time.timeScale = 0;
                OnGamePauseChange?.Invoke(true);
                break;
            case GameState.Paused:
                state = GameState.Playing;
                Time.timeScale = 1;
                OnGamePauseChange?.Invoke(false);
                break;
            case GameState.GameOver:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void OnPlayerDeath()
    {
        state = GameState.GameOver;
    }
}
