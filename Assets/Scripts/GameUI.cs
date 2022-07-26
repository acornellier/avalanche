using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] Text highscoreText;
    [SerializeField] Text heightText;
    [SerializeField] Transform player;

    float _highscore;
    float _initialHeight;

    void Start()
    {
        _initialHeight = player.position.y;
    }

    void Update()
    {
        var curHeight = Mathf.Round(player.position.y - _initialHeight);
        heightText.text = $"{curHeight}ft";
        if (player.position.y <= _highscore)
            return;

        _highscore = player.position.y;
        highscoreText.text = $"{Mathf.Round(_highscore)}ft";
    }
}
