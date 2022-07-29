using UnityEngine;
using Zenject;

public class Menu : MonoBehaviour
{
    [Inject] protected MenuManager menuManager;

    public void GoBack()
    {
        menuManager.GoBack();
    }

    public void OpenMenu(GameObject menu)
    {
        menuManager.OpenMenu(menu);
    }
}
