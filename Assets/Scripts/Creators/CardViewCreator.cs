using UnityEngine;
using DG.Tweening; // Needed for DOScale

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;

    public CardView CreateCardView(Vector3 position, Quaternion rotation)
    {
        // Instantiate card view at the specified position and rotation
        CardView cardView = Instantiate(cardViewPrefab, position, rotation);
        
        // Animate card appearing
        cardView.transform.localScale = Vector3.zero;
        cardView.transform
            .DOScale(Vector3.one, 0.15f)
            .SetEase(Ease.OutBack);

        var randomizer = cardView.GetComponent<RandomizeImage>();
        if (randomizer != null)
            randomizer.InitializeRandomCard(); // <-- Ensure this is called!

        return cardView;
    }
}
