using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject pauseMenuUi;
    [SerializeField] GameObject optionsMenuUi;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _gameManager.TogglePause();
    }

    void PauseCallback()
    {
        pausePanel.SetActive(true);
        pauseMenuUi.SetActive(true);
        optionsMenuUi.SetActive(false);
    }

    void ResumeCallback()
    {
        pausePanel.SetActive(false);
    }

    public void Resume()
    {
        if (_gameManager.state == GameState.Paused)
            _gameManager.TogglePause();
    }

    public void Options()
    {
        pauseMenuUi.SetActive(false);
        optionsMenuUi.SetActive(true);
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
