using UnityEngine;
using TMPro;

public class RandomizeNumber : MonoBehaviour
{
    [SerializeField] public TMP_Text textComponent;

    private void Start()
    {
        int randomNumber = Random.Range(1, 10); // 1 to 10 inclusive
        if (textComponent != null)
            textComponent.text = randomNumber.ToString();
    }
}
