using UnityEngine;
using DG.Tweening;
using TMPro;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text cardName;    // Assign CardName TMP_Text in Inspector
    [SerializeField] private TMP_Text numberText;  // Assign CardNumber TMP_Text in Inspector
    public Sprite currentSprite;

    private HandView handView;
    public void SetHandView(HandView hv) => handView = hv;

    public string cardNameValue
    {
        get
        {
            if (cardName != null)
                return cardName.text;
            return "Unknown";
        }
    }

    public int cardNumber
    {
        get
        {
            if (numberText != null && int.TryParse(numberText.text, out int num))
                return num;
            return 0;
        }
    }

    private void OnMouseEnter()
    {
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }

    private void OnMouseExit()
    {
        transform.DOScale(0.8f, 0.2f).SetEase(Ease.OutBack);
    }

    private void OnMouseDown()
    {
        if (handView != null)
            handView.SelectPlayerCard(this);
    }
}
