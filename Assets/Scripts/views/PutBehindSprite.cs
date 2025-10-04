using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Background";
            sr.sortingOrder = -1;
        }
    }
}
