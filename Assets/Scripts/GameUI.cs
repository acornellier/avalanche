using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
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
    }
}
