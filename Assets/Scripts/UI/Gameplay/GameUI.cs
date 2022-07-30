using TMPro;
using UnityEngine;
using Zenject;

public class GameUI : MonoBehaviour
{
    [SerializeField] TMP_Text highscoreText;
    [SerializeField] TMP_Text heightText;

    [Inject] HeightTracker _heightTracker;

    void LateUpdate()
    {
        heightText.text = $"{_heightTracker.currentHeight}ft";
        highscoreText.text = $"{Mathf.Round(_heightTracker.maxHeight)}ft";
    }
}
