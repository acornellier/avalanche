using System.Collections.Generic;
using UnityEngine;

public class MenuManager
{
    readonly Stack<GameObject> _menuStack = new();

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

    public void GoBack()
    {
        if (_menuStack.TryPop(out var oldMenu))
            oldMenu.SetActive(false);

        if (_menuStack.TryPeek(out var newMenu))
            newMenu.SetActive(true);
    }
}
