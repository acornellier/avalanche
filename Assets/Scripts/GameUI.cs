using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] Text highscoreText;
    [SerializeField] Text heightText;
    [SerializeField] Transform player;

    float _highestPoint;
    float _lowestPoint;

    void Update()
    {
        if (player.position.y < _lowestPoint)
            _lowestPoint = player.position.y;

        var curHeight = Mathf.Round(player.position.y - _lowestPoint);
        heightText.text = $"{curHeight}ft";
        if (curHeight <= _highestPoint)
            return;

        _highestPoint = curHeight;
        highscoreText.text = $"{Mathf.Round(_highestPoint)}ft";
    }
}
