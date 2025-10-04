using UnityEngine;
using UnityEngine.UI;

public class GameoverLogic : MonoBehaviour
{
    [SerializeField] public Image gameFinishedImage; // Drag your UI Image here in Inspector

    public void ShowGameFinished()
    {
        if (gameFinishedImage != null)
            gameFinishedImage.enabled = true;
    }
}