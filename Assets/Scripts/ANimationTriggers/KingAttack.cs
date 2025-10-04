// ===== KING ATTACK (UI Display Version) =====
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class KingAttack : MonoBehaviour
{
    [Header("Single Video Player")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Video Clips")]
    [SerializeField] private VideoClip katanaAttackClip;
    [SerializeField] private VideoClip rapierAttackClip;
    [SerializeField] private VideoClip claymoreAttackClip;

    [Header("UI Display")]
    [SerializeField] private RawImage videoDisplay; // The UI RawImage to show video

    [Header("Sprite Components (for idle)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator spriteAnimator;

    private RenderTexture renderTexture;

    private void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError($"Video Player not assigned on {gameObject.name}!");
            return;
        }

        // Create render texture
        renderTexture = new RenderTexture(1920, 1080, 0);

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.loopPointReached += OnVideoFinished;

        // Assign render texture to UI
        if (videoDisplay != null)
        {
            videoDisplay.texture = renderTexture;
            videoDisplay.gameObject.SetActive(false); // Hide initially
        }

        Debug.Log($"KingAttack initialized on {gameObject.name}");
    }

    public void KingKatanaSlash()
    {
        Debug.Log("KingKatanaSlash called");
        PlayAttackVideo(katanaAttackClip);
    }

    public void KingRapierSlash()
    {
        Debug.Log("KingRapierSlash called");
        PlayAttackVideo(rapierAttackClip);
    }

    public void KingClaymoreSlash()
    {
        Debug.Log("KingClaymoreSlash called");
        PlayAttackVideo(claymoreAttackClip);
    }

    private void PlayAttackVideo(VideoClip clip)
    {
        if (videoPlayer == null || clip == null)
        {
            Debug.LogError($"Video Player or Clip missing on {gameObject.name}!");
            return;
        }

        Debug.Log($"Playing video: {clip.name}");

        // Show video display
        if (videoDisplay != null)
        {
            videoDisplay.gameObject.SetActive(true);
        }

        // Hide sprite (optional)
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (spriteAnimator != null) spriteAnimator.enabled = false;

        // Play video
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log($"Video finished on {gameObject.name}");

        // Hide video display
        if (videoDisplay != null)
        {
            videoDisplay.gameObject.SetActive(false);
        }

        // Show sprite again
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (spriteAnimator != null) spriteAnimator.enabled = true;
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;

        if (renderTexture != null)
            Destroy(renderTexture);
    }
}