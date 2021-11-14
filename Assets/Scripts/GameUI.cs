using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    float highscore;
    public Text highscoreText;
    public Text heightText;
    public Transform player;

    float initialHeight;

    void Start()
    {
        initialHeight = player.position.y;
    }

    void Update()
    {
        var curHeight = Mathf.Round(player.position.y - initialHeight);
        heightText.text = $"{curHeight}ft";
        if (player.position.y > highscore) {
            highscore = player.position.y;
            highscoreText.text = player.position.y.ToString();
        }
    }
}
