using System;
using UnityEngine;
using Zenject;

public enum GameState
{
    Playing,
    Paused,
    GameOver,
}

public class GameManager : IInitializable
{
    [InjectOptional] Player _player;
    [Inject] public bool isGameScene;

    public GameState state { get; private set; } = GameState.Playing;
    public event Action<bool> OnGamePauseChange;

    public void Initialize()
    {
        Time.timeScale = 1;
        if (_player)
            _player.OnPlayerDeath += OnPlayerDeath;
    }

    void OnPlayerDeath()
    {
        SetState(GameState.GameOver);
    }

    public void SetState(GameState newState)
    {
        if (newState == GameState.Paused)
        {
            if (state == GameState.GameOver)
                return;

            Time.timeScale = 0;
            OnGamePauseChange?.Invoke(true);
        }
        else if (state == GameState.Paused)
        {
            Time.timeScale = 1;
            OnGamePauseChange?.Invoke(false);
        }

        state = newState;
    }
}
