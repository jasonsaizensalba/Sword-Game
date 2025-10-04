using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;  // Assign your HandView in Inspector

    private void Update()
    {
        // Optional: spawn a new card for testing with Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Spawn a card using your existing RandomizeImage system.");
            // You can call your existing RandomizeImage.SpawnRandomCard() here if you want
        }
    }
}
    