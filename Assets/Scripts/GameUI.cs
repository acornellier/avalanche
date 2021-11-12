using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Text heightText;
    public Transform player;

    void Update()
    {
        heightText.text = $"{Mathf.Round(player.position.y)}ft";
    }
}
