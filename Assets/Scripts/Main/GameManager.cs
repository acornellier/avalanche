using System;
using Zenject;

public enum GameState
{
    Playing,
    Paused,
    GameOver,
}

public class GameManager : IInitializable
{
    public GameState state { get; private set; } = GameState.Playing;

    [Inject] Player _player;

    public event Action<bool> OnGamePauseChange;

    public void Initialize()
    {
        _player.OnPlayerDeath += OnPlayerDeath;
    }

    public void TogglePause()
    {
        switch (state)
        {
            case GameState.Playing:
                state = GameState.Paused;
                OnGamePauseChange?.Invoke(true);
                break;
            case GameState.Paused:
                state = GameState.Playing;
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
