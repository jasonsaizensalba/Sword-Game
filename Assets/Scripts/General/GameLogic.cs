using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CardView playerCard;
    private CardView enemyCard;

    private PlayAnimation playAnim;

    void Start()
    {
        playAnim = FindObjectOfType<PlayAnimation>();
   
    }

    // Call this when the player selects a card
    public void SetPlayerCard(CardView card)
    {
        playerCard = card;
    }

    // Call this when the enemy card is spawned
    public void SetEnemyCard(CardView card)
    {
        enemyCard = card;
    }

    // Call this to resolve the battle
    public void Battle()
    {
        if (playerCard == null || enemyCard == null)
        {
            Debug.LogWarning("Both cards must be set before battling!");
            return;
        }

        string playerType = playerCard.cardNameValue;
        string enemyType = enemyCard.cardNameValue;
        int playerNumber = playerCard.cardNumber;
        int enemyNumber = enemyCard.cardNumber;

        Debug.Log($"Player: {playerType} {playerNumber}");
        Debug.Log($"Enemy: {enemyType} {enemyNumber}");

        // If same type, higher number wins

        if (playerType == enemyType)
        {
            if (playerNumber == enemyNumber)
            {
                Debug.Log("It's a tie!");

            }
        }
        else if (playerType == "Katana" && enemyType == "Rapier")
        {
            if (playerNumber == enemyNumber)
            {
                Debug.Log("It's a tie!");

            }
            else if (playerNumber > enemyNumber)
            {
                Debug.Log("Player wins!");
                if (playAnim != null)
                { 
                playAnim.Katana();
                }

            }
                else
                {
                    Debug.Log("Enemy wins!");
                }
        }
        else if (playerType == "Rapier" && enemyType == "Claymore")
        {
            if (playerNumber == enemyNumber)
            {
                Debug.Log("It's a tie!");

            }
            else if (playerNumber > enemyNumber)
            {
                Debug.Log("Player wins!");
               

            }
            else
            {
                Debug.Log("Enemy wins!");
            }
        }
        else if (playerType == "Claymore" && enemyType == "Katana")
        {
            if (playerNumber == enemyNumber)
            {
                Debug.Log("It's a tie!");

            }
            else if (playerNumber > enemyNumber)
            {
                Debug.Log("Player wins!");
                

            }
            else
            {
                Debug.Log("Enemy wins!");
            }
        }
    }
    
}
