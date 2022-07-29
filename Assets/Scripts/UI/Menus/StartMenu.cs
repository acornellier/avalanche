using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : Menu
{
    [SerializeField] GameObject helpUi;

    void Awake()
    {
        menuManager.OpenMenu(gameObject);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        GameUtilities.Quit();
    }
}
