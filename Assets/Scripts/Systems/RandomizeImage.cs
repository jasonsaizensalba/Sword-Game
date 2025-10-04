using UnityEngine;
using TMPro;

public class RandomizeImage : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] sprites; // Katana, Rapier, Claymore

    [Header("UI References")]
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text numberText;

    [Header("Transform Settings")]
    [SerializeField] private Vector3 uniformScale = new Vector3(0.4267421f, 0.4166843f, 1f);
    [SerializeField] private Vector3 uniformPosition = new Vector3(-0.07f, 0.06f, 0f);

    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Only assign if not set in Inspector
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        InitializeRandomCard();
    }

    public void InitializeRandomCard()
    {
        if (sprites.Length == 0)
        {
            Debug.LogWarning("No sprites assigned in RandomizeImage!");
            return;
        }

        int randIndex = Random.Range(0, sprites.Length);
        Sprite chosenSprite = sprites[randIndex];
        spriteRenderer.sprite = chosenSprite;

        // Clean up the name for display
        string cleanName = chosenSprite.name;
        if (cleanName.Contains("_"))
            cleanName = cleanName.Split('_')[0];

        if (cardNameText != null)
            cardNameText.text = cleanName;

        // Set uniform scale and position
        transform.localScale = uniformScale;
        transform.localPosition = uniformPosition;
    }

    public Sprite GetCurrentSprite()
    {
        return spriteRenderer != null ? spriteRenderer.sprite : null;
    }


}
