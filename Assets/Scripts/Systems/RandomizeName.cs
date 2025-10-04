using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CardNameRandomizer : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private Image imageComponent;           // UI Image (uGUI)
    [SerializeField] private SpriteRenderer spriteRenderer;  // optional (world sprite)
    [SerializeField] private int maxWaitFrames = 5;          // how many frames to wait for sprite assignment

    private void Reset()
    {
        // Auto-assign common components if not set in Inspector
        if (textComponent == null) textComponent = GetComponentInChildren<TMP_Text>();
        if (imageComponent == null) imageComponent = GetComponentInChildren<Image>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // Try to pick up the name after a few frames if sprite is assigned by another script
        StartCoroutine(EnsureNameMatchesSprite());
    }

    private IEnumerator EnsureNameMatchesSprite()
    {
        Sprite s = GetCurrentSprite();
        int tries = 0;

        // wait up to maxWaitFrames frames for the sprite to be set by other scripts
        while (s == null && tries < maxWaitFrames)
        {
            tries++;
            yield return null; // wait one frame
            s = GetCurrentSprite();
        }

        SetCardNameFromSprite(s);
    }

    // Call this from RandomizeImage immediately after you set the sprite
    public void RefreshName()
    {
        SetCardNameFromSprite(GetCurrentSprite());
    }

    private Sprite GetCurrentSprite()
    {
        if (imageComponent != null && imageComponent.sprite != null) return imageComponent.sprite;
        if (spriteRenderer != null && spriteRenderer.sprite != null) return spriteRenderer.sprite;
        return null;
    }

    private void SetCardNameFromSprite(Sprite s)
    {
        if (textComponent == null)
            textComponent = GetComponentInChildren<TMP_Text>();

        if (textComponent == null) return;
        if (s == null) return;

        string cleanName = s.name;

        // Remove everything after the first underscore (e.g., "Katana_0" -> "Katana")
        if (cleanName.Contains("_"))
        {
            cleanName = cleanName.Split('_')[0];
        }

        textComponent.text = cleanName;
    }
}

public class RandomizeName : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private SpriteRenderer spriteRenderer; // Drag your Square SpriteRenderer here in the Inspector

    public string cardName; // This is the card's type as a string

    private void Start()
    {
        UpdateCardNameFromSprite();
    }

    // Call this after you change the sprite (e.g., after randomizing)
    public void UpdateCardNameFromSprite()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        string cleanName = spriteRenderer.sprite.name;
        // Remove everything after the first underscore (e.g., "Katana_0" -> "Katana")
        if (cleanName.Contains("_"))
            cleanName = cleanName.Split('_')[0];

        cardName = cleanName;
        if (nameText != null) nameText.text = cardName;
    }
}
