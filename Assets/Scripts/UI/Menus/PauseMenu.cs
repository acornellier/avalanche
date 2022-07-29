using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class PauseMenu : Menu
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject pauseMenuUi;

    [Inject] GameManager _gameManager;

    void OnEnable()
    {
        _gameManager.OnGamePauseChange += OnGamePauseChange;
    }

    void OnDisable()
    {
        _gameManager.OnGamePauseChange -= OnGamePauseChange;
    }

    void OnGamePauseChange(bool paused)
    {
        if (paused) PauseCallback();
        else ResumeCallback();
    }

    void PauseCallback()
    {
        menuManager.CloseAll();
        pausePanel.SetActive(true);
        menuManager.OpenMenu(pauseMenuUi);
    }

    void ResumeCallback()
    {
        menuManager.CloseAll();
        pausePanel.SetActive(false);
    }

    public void Resume()
    {
        if (_gameManager.state == GameState.Paused)
            _gameManager.TogglePause();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void Quit()
    {
        GameUtilities.Quit();
    }
}
