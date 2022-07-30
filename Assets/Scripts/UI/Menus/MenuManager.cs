using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MenuManager : ITickable
{
    [Inject] GameManager _gameManager;

    readonly Stack<GameObject> _menuStack = new();

    public void Tick()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (_gameManager.state != GameState.Paused)
            _gameManager.SetState(GameState.Paused);
        else
            GoBackOrResume();
    }

    public void CloseAll()
    {
        while (_menuStack.TryPop(out var menu))
        {
            menu.SetActive(false);
        }
    }

    public void OpenMenu(GameObject menu)
    {
        if (_menuStack.TryPeek(out var oldMenu))
            oldMenu.SetActive(false);

        _menuStack.Push(menu);
        menu.SetActive(true);
    }

    public void GoBackOrResume()
    {
        GoBack();
        if (_menuStack.Count <= 0)
            _gameManager.SetState(GameState.Playing);
    }

    public void GoBack()
    {
        if (_menuStack.Count == 1 && !_gameManager.isGameScene)
            return;

        if (_menuStack.TryPop(out var oldMenu))
            oldMenu.SetActive(false);

        if (_menuStack.TryPeek(out var newMenu))
            newMenu.SetActive(true);
    }
}
