using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameUI : MonoBehaviour
{
    [SerializeField] Text highscoreText;
    [SerializeField] Text heightText;

    [Inject] HeightTracker _heightTracker;

    void LateUpdate()
    {
        heightText.text = $"{_heightTracker.currentHeight}ft";
        highscoreText.text = $"{Mathf.Round(_heightTracker.maxHeight)}ft";
    }
}
