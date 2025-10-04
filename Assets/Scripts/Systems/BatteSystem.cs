using UnityEngine;

public static class BattleSystem
{
    public static CardView GetWinner(CardView playerCard, CardView enemyCard)
    {
        if (playerCard == null || enemyCard == null) return null;

        string playerType = playerCard.cardNameValue;
        string enemyType = enemyCard.cardNameValue;
        int playerNumber = playerCard.cardNumber;
        int enemyNumber = enemyCard.cardNumber;

        Debug.Log($"Player: {playerType} {playerNumber}");
        Debug.Log($"Enemy: {enemyType} {enemyNumber}");

        // If same type, higher number wins
        if (playerType == enemyType)
        {
            if (playerNumber > enemyNumber) return playerCard;
            if (enemyNumber > playerNumber) return enemyCard;
            return null; // tie
        }

        // Type advantage: Katana > Rapier > Claymore > Katana
        if ((playerType == "Katana" && enemyType == "Rapier") ||
            (playerType == "Rapier" && enemyType == "Claymore") ||
            (playerType == "Claymore" && enemyType == "Katana"))
            return playerCard;

        if ((enemyType == "Katana" && playerType == "Rapier") ||
            (enemyType == "Rapier" && playerType == "Claymore") ||
            (enemyType == "Claymore" && playerType == "Katana"))
            return enemyCard;

        return null; // fallback tie
    }
}
