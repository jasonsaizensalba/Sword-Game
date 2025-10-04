using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.UI;
using UnityEngine.Video;

public class HandView : MonoBehaviour
{
    [Header("Spline Settings")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float cardSpacing = 0.15f;
    [SerializeField] private float cardScale = 0.8f;
    [SerializeField] private float moveDuration = 0.5f;

    [Header("Prefab Settings")]
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private CardView enemyCardPrefab;

    private readonly List<CardView> cards = new List<CardView>();
    private CardView playerCard;
    private CardView enemyCard;

    [Header("Scores")]
    public int playerScore = 0;
    public int enemyScore = 0;

    // Track wins by card type
    [Header("Detailed Win Tracking")]
    public int playerKatanaWins = 0;
    public int playerRapierWins = 0;
    public int playerClaymoreWins = 0;

    public int enemyKatanaWins = 0;
    public int enemyRapierWins = 0;
    public int enemyClaymoreWins = 0;

    [Header("Timer Settings")]
    [SerializeField] private float roundTimeLimit = 15f;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    private float currentTime;
    private bool timerActive = false;

    [Header("Win Display UI")]
    [SerializeField] private TMPro.TextMeshProUGUI winDisplayText;
    [SerializeField] private float winDisplayDuration = 2f;

    [Header("Card Stack Display")]
    [SerializeField] private RectTransform heroCardStack; // UI RectTransform for hero stack
    [SerializeField] private RectTransform kingCardStack; // UI RectTransform for king stack
    [SerializeField] private GameObject cardStackPrefab; // UI prefab with Image component
    [SerializeField] private float stackOffsetY = -30f; // Vertical spacing
    [SerializeField] private Vector2 stackCardSize = new Vector2(80, 112); // Size of stacked cards

    private List<GameObject> heroStackedCards = new List<GameObject>();
    private List<GameObject> kingStackedCards = new List<GameObject>();

    private bool isProcessingBattle = false;

    public bool isDone = false;
    public GameObject gameOverUI;

    private void Start()
    {
        if (winDisplayText != null)
            winDisplayText.gameObject.SetActive(false);

        StartCoroutine(GiveInitialCards());
    }

    private void Update()
    {
        if (timerActive)
        {
            currentTime -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = $"Time: {Mathf.Ceil(currentTime)}s";
                timerText.color = currentTime <= 5f ? Color.red : Color.white;
            }

            if (currentTime <= 0f)
            {
                timerActive = false;
                TimeOut();
            }
        }
    }

    private IEnumerator GiveInitialCards()
    {
        for (int i = 0; i < 5; i++)
        {
            CardView card = Instantiate(cardPrefab, transform.position, Quaternion.identity);
            card.SetHandView(this);

            RandomizeImage ri = card.GetComponent<RandomizeImage>();
            if (ri != null)
                ri.InitializeRandomCard();

            cards.Add(card);
            yield return StartCoroutine(UpdateCardPositions(moveDuration));
        }

        StartTimer();
    }

    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0) yield break;

        Spline spline = splineContainer.Spline;
        float startPos = 0.5f - ((cards.Count - 1) * cardSpacing / 2f);

        for (int i = 0; i < cards.Count; i++)
        {
            float t = startPos + i * cardSpacing;
            Vector3 targetPos = splineContainer.transform.TransformPoint(spline.EvaluatePosition(t));
            targetPos += Vector3.back * i * 0.01f;

            Quaternion rotation = Quaternion.LookRotation(targetPos - Camera.main.transform.position, Vector3.up);

            cards[i].transform.DOMove(targetPos, duration).SetEase(Ease.OutQuad);
            cards[i].transform.DORotateQuaternion(rotation, duration).SetEase(Ease.OutQuad);
            cards[i].transform.DOScale(Vector3.one * cardScale, duration).SetEase(Ease.OutQuad);
        }

        yield return new WaitForSeconds(duration);
    }

    public void SelectPlayerCard(CardView card)
    {
        if (playerCard != null || isProcessingBattle) return;

        StopTimer();
        isProcessingBattle = true;
        playerCard = card;
        cards.Remove(card);

        Vector3 battlePos = new Vector3(-3.64f, 0.38f, 0f);
        playerCard.transform.DOMove(battlePos, 0.5f).SetEase(Ease.OutQuad);
        playerCard.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.OutBack);

        DOVirtual.DelayedCall(0.7f, SpawnEnemyCard);
    }

    private void SpawnEnemyCard()
    {
        Vector3 startPos = new Vector3(8f, 0.38f, 0f);
        Vector3 battlePos = new Vector3(3.64f, 0.38f, 0f);

        enemyCard = Instantiate(enemyCardPrefab, startPos, Quaternion.identity);
        enemyCard.SetHandView(this);

        RandomizeImage ri = enemyCard.GetComponent<RandomizeImage>();
        if (ri != null)
            ri.InitializeRandomCard();

        enemyCard.transform.localScale = Vector3.one * cardScale;
        enemyCard.transform.DOMove(battlePos, 0.5f).SetEase(Ease.OutQuad);
        enemyCard.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.OutBack);

        DOVirtual.DelayedCall(0.6f, ResolveBattle);
    }

    private void ResolveBattle()
    {
        CardView winner = BattleSystem.GetWinner(playerCard, enemyCard);

        string playerType = playerCard.cardNameValue;
        string enemyType = enemyCard.cardNameValue;

        float videoDelay = 0f;

        if (winner == playerCard)
        {
            playerScore++;
            Debug.Log($"Player wins! Score: {playerScore}");

            HeroAttack heroAttack = FindObjectOfType<HeroAttack>();
            if (heroAttack != null)
            {
                if (playerType == "Katana")
                {
                    playerKatanaWins++;
                    heroAttack.HeroKatanaSlash();
                    videoDelay = GetVideoLength(heroAttack);
                    ShowWinDisplay($"KATANA WINS!\nPlayer: {playerKatanaWins}/3");
                    AddCardToStack(playerCard, heroCardStack, heroStackedCards);
                }
                else if (playerType == "Rapier")
                {
                    playerRapierWins++;
                    heroAttack.HeroRapierSlash();
                    videoDelay = GetVideoLength(heroAttack);
                    ShowWinDisplay($"RAPIER WINS!\nPlayer: {playerRapierWins}/3");
                    AddCardToStack(playerCard, heroCardStack, heroStackedCards);
                }
                else if (playerType == "Claymore")
                {
                    playerClaymoreWins++;
                    heroAttack.HeroClaymoreSlash();
                    videoDelay = GetVideoLength(heroAttack);
                    ShowWinDisplay($"CLAYMORE WINS!\nPlayer: {playerClaymoreWins}/3");
                    AddCardToStack(playerCard, heroCardStack, heroStackedCards);
                }
            }
        }
        else if (winner == enemyCard)
        {
            enemyScore++;
            Debug.Log($"Enemy wins! Score: {enemyScore}");

            KingAttack kingAttack = FindObjectOfType<KingAttack>();
            if (kingAttack != null)
            {
                if (enemyType == "Katana")
                {
                    enemyKatanaWins++;
                    kingAttack.KingKatanaSlash();
                    videoDelay = GetVideoLength(kingAttack);
                    ShowWinDisplay($"KATANA WINS!\nKing: {enemyKatanaWins}/3");
                    AddCardToStack(enemyCard, kingCardStack, kingStackedCards);
                }
                else if (enemyType == "Rapier")
                {
                    enemyRapierWins++;
                    kingAttack.KingRapierSlash();
                    videoDelay = GetVideoLength(kingAttack);
                    ShowWinDisplay($"RAPIER WINS!\nKing: {enemyRapierWins}/3");
                    AddCardToStack(enemyCard, kingCardStack, kingStackedCards);
                }
                else if (enemyType == "Claymore")
                {
                    enemyClaymoreWins++;
                    kingAttack.KingClaymoreSlash();
                    videoDelay = GetVideoLength(kingAttack);
                    ShowWinDisplay($"CLAYMORE WINS!\nKing: {enemyClaymoreWins}/3");
                    AddCardToStack(enemyCard, kingCardStack, kingStackedCards);
                }
            }
        }
        else
        {
            Debug.Log("It's a tie!");
        }

        if (playerKatanaWins == 3 || playerRapierWins == 3 || playerClaymoreWins == 3 && !isDone)
        {
            isDone = true;
            Debug.Log("Player has won the game!");
            gameOver();
            
            return;
        }
        else if (enemyKatanaWins == 3 || enemyRapierWins == 3 || enemyClaymoreWins == 3 && !isDone)
        {
            isDone = true;
            Debug.Log("Enemy has won the game!");
            gameOver();
            
            return;
        }

        float totalDelay = Mathf.Max(1f, videoDelay + 0.5f);
        DOVirtual.DelayedCall(totalDelay, ResetRound);
    }

    private float GetVideoLength(MonoBehaviour attackScript)
    {
        VideoPlayer vp = attackScript.GetComponent<VideoPlayer>();
        if (vp != null && vp.clip != null)
        {
            return (float)vp.clip.length;
        }
        return 2f;
    }

    private void ResetRound()
    {
        if (playerCard != null) Destroy(playerCard.gameObject);
        if (enemyCard != null) Destroy(enemyCard.gameObject);

        playerCard = null;
        enemyCard = null;
        isProcessingBattle = false;

        foreach (var c in cards) Destroy(c.gameObject);
        cards.Clear();

        StartCoroutine(GiveInitialCards());
    }

    public string PlayerCardInfo =>
        playerCard != null ? $"{playerCard.cardNameValue} {playerCard.cardNumber}" : "None";

    public string EnemyCardInfo =>
        enemyCard != null ? $"{enemyCard.cardNameValue} {enemyCard.cardNumber}" : "None";

    private void StartTimer()
    {
        currentTime = roundTimeLimit;
        timerActive = true;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.color = Color.white;
        }
    }

    private void StopTimer()
    {
        timerActive = false;
        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    private void TimeOut()
    {
        if (isProcessingBattle) return;

        Debug.Log("Time's up! King wins by default.");
        isProcessingBattle = true;

        // Spawn enemy card at battle position
        Vector3 startPos = new Vector3(8f, 0.38f, 0f);
        Vector3 battlePos = new Vector3(3.64f, 0.38f, 0f);

        enemyCard = Instantiate(enemyCardPrefab, startPos, Quaternion.identity);
        enemyCard.SetHandView(this);

        RandomizeImage ri = enemyCard.GetComponent<RandomizeImage>();
        if (ri != null)
            ri.InitializeRandomCard();

        enemyCard.transform.localScale = Vector3.one * cardScale;
        enemyCard.transform.DOMove(battlePos, 0.5f).SetEase(Ease.OutQuad);
        enemyCard.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.OutBack);

        // Wait for card animation to complete, then resolve timeout
        DOVirtual.DelayedCall(0.6f, ResolveTimeout);
    }

    private void ResolveTimeout()
    {
        enemyScore++;
        
        string enemyType = enemyCard.cardNameValue;
        float videoDelay = 0f;

        KingAttack kingAttack = FindObjectOfType<KingAttack>();
        
        if (kingAttack != null)
        {
            if (enemyType == "Katana")
            {
                enemyKatanaWins++;
                kingAttack.KingKatanaSlash();
                videoDelay = GetVideoLength(kingAttack);
                ShowWinDisplay($"TIME OUT!\nKATANA WINS!\nKing: {enemyKatanaWins}/3");
                AddCardToStack(enemyCard, kingCardStack, kingStackedCards);
            }
            else if (enemyType == "Rapier")
            {
                enemyRapierWins++;
                kingAttack.KingRapierSlash();
                videoDelay = GetVideoLength(kingAttack);
                ShowWinDisplay($"TIME OUT!\nRAPIER WINS!\nKing: {enemyRapierWins}/3");
                AddCardToStack(enemyCard, kingCardStack, kingStackedCards);
            }
            else if (enemyType == "Claymore")
            {
                enemyClaymoreWins++;
                kingAttack.KingClaymoreSlash();
                videoDelay = GetVideoLength(kingAttack);
                ShowWinDisplay($"TIME OUT!\nCLAYMORE WINS!\nKing: {enemyClaymoreWins}/3");
                AddCardToStack(enemyCard, kingCardStack, kingStackedCards);
            }
        }

        // Check game over
        if (enemyKatanaWins == 3 || enemyRapierWins == 3 || enemyClaymoreWins == 3)
        {
            Debug.Log("Enemy has won the game!");
            FindObjectOfType<GameoverLogic>().ShowGameFinished();
            return;
        }

        // Wait for video to finish before resetting
        float totalDelay = Mathf.Max(1f, videoDelay + 0.5f);
        DOVirtual.DelayedCall(totalDelay, ResetRound);
    }

    private void ShowWinDisplay(string message)
    {
        if (winDisplayText != null)
        {
            winDisplayText.text = message;
            winDisplayText.gameObject.SetActive(true);

            DOVirtual.DelayedCall(winDisplayDuration, () => {
                if (winDisplayText != null)
                    winDisplayText.gameObject.SetActive(false);
            });
        }
    }

    private void AddCardToStack(CardView winningCard, RectTransform stackParent, List<GameObject> stackList)
    {
        if (stackParent == null)
        {
            Debug.LogError("Stack parent is NULL! Please assign Hero Card Stack and King Card Stack in the Inspector.");
            return;
        }

        if (winningCard == null)
        {
            Debug.LogWarning("Winning card is null!");
            return;
        }

        Debug.Log($"Adding card to stack: {stackParent.name}, Card type: {winningCard.cardNameValue}");

        // Get the card's sprite - check all possible sources
        Sprite cardSprite = null;
        
        // Try SpriteRenderer first
        SpriteRenderer sr = winningCard.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            cardSprite = sr.sprite;
            Debug.Log($"Got sprite from SpriteRenderer: {cardSprite.name}");
        }
        
        // Try Image component if SpriteRenderer didn't work
        if (cardSprite == null)
        {
            Image cardImg = winningCard.GetComponent<Image>();
            if (cardImg != null && cardImg.sprite != null)
            {
                cardSprite = cardImg.sprite;
                Debug.Log($"Got sprite from Image: {cardSprite.name}");
            }
        }
        
        // Check children for sprite renderer
        if (cardSprite == null)
        {
            SpriteRenderer[] childRenderers = winningCard.GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in childRenderers)
            {
                if (renderer.sprite != null && renderer.gameObject.name.Contains("Card"))
                {
                    cardSprite = renderer.sprite;
                    Debug.Log($"Got sprite from child: {renderer.gameObject.name}");
                    break;
                }
            }
        }

        if (cardSprite == null)
        {
            Debug.LogWarning($"No sprite found for card! Checking RandomizeImage component...");
            RandomizeImage ri = winningCard.GetComponent<RandomizeImage>();
            if (ri != null)
            {
                // Try to get the current sprite from RandomizeImage
                SpriteRenderer riSr = ri.GetComponent<SpriteRenderer>();
                if (riSr != null)
                {
                    cardSprite = riSr.sprite;
                    Debug.Log($"Got sprite from RandomizeImage SpriteRenderer: {cardSprite?.name ?? "null"}");
                }
            }
        }

        // Create UI card
        GameObject stackCard;
        Image cardImage;

        if (cardStackPrefab != null)
        {
            Debug.Log("Using card stack prefab");
            stackCard = Instantiate(cardStackPrefab, stackParent);
            cardImage = stackCard.GetComponent<Image>();
            if (cardImage == null)
            {
                Debug.LogError("Card stack prefab must have an Image component!");
                Destroy(stackCard);
                return;
            }
        }
        else
        {
            Debug.Log("Creating default UI card");
            // Create default UI card
            stackCard = new GameObject("StackedCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            stackCard.transform.SetParent(stackParent, false);
            cardImage = stackCard.GetComponent<Image>();
        }

        // Set sprite and color
        if (cardSprite != null)
        {
            cardImage.sprite = cardSprite;
            cardImage.color = Color.white;
            Debug.Log($"✓ Sprite applied successfully: {cardSprite.name}");
        }
        else
        {
            // Create a colored card based on card type as fallback
            cardImage.sprite = null;
            
            string cardType = winningCard.cardNameValue;
            if (cardType == "Katana")
                cardImage.color = new Color(1f, 0.3f, 0.3f); // Red
            else if (cardType == "Rapier")
                cardImage.color = new Color(0.3f, 0.3f, 1f); // Blue
            else if (cardType == "Claymore")
                cardImage.color = new Color(0.3f, 1f, 0.3f); // Green
            else
                cardImage.color = new Color(1f, 0.8f, 0.2f); // Orange default
                
            Debug.LogWarning($"Using colored rectangle for {cardType} card (sprite not found)");
            
            // Add text label to show card type
            GameObject textObj = new GameObject("CardLabel");
            textObj.transform.SetParent(stackCard.transform, false);
            TMPro.TextMeshProUGUI label = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            label.text = cardType;
            label.fontSize = 14;
            label.alignment = TMPro.TextAlignmentOptions.Center;
            label.color = Color.white;
            
            RectTransform labelRect = textObj.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
        }

        // Setup RectTransform
        RectTransform rt = stackCard.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = stackCardSize;
        rt.localScale = Vector3.one;
        
        // Position in stack (each card goes below the previous one)
        float yOffset = stackList.Count * stackOffsetY;
        rt.anchoredPosition = new Vector2(0, yOffset);

        // Animate entrance
        rt.localScale = Vector3.zero;
        rt.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        stackList.Add(stackCard);

        Debug.Log($"✓ Successfully added card to {stackParent.name} stack. Total cards: {stackList.Count}");
    }

    private void ClearCardStacks()
    {
        foreach (var card in heroStackedCards)
        {
            if (card != null) Destroy(card);
        }
        heroStackedCards.Clear();

        foreach (var card in kingStackedCards)
        {
            if (card != null) Destroy(card);
        }
        kingStackedCards.Clear();
    }

    public void gameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void restart()
    {
        SceneManager.LoadScene("Battle");
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void quit()
    {
        Application.Quit();
    }
}