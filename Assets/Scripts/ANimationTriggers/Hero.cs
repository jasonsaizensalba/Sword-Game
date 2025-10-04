using UnityEngine;
using UnityEngine.Video;

// ===== HERO ATTACK (Simplified) =====
public class Hero : MonoBehaviour
{
    [Header("Single Video Player")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Video Clips")]
    [SerializeField] private VideoClip katanaAttackClip;
    [SerializeField] private VideoClip rapierAttackClip;
    [SerializeField] private VideoClip claymoreAttackClip;

    [Header("Sprite Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator spriteAnimator;

    [Header("Video Display")]
    [SerializeField] private RenderTexture renderTexture;
    private Material videoMaterial;
    private Sprite originalSprite;
    private bool isPlayingVideo = false;

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;

            // Use RenderTexture mode instead of CameraNearPlane
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;

            // Create RenderTexture if not assigned
            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(1920, 1080, 0);
            }
            videoPlayer.targetTexture = renderTexture;

            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }

        // Store original sprite
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
    }

    public void HeroKatanaSlash()
    {
        PlayAttackVideo(katanaAttackClip);
    }

    public void HeroRapierSlash()
    {
        PlayAttackVideo(rapierAttackClip);
    }

    public void HeroClaymoreSlash()
    {
        PlayAttackVideo(claymoreAttackClip);
    }

    private void PlayAttackVideo(VideoClip clip)
    {
        if (videoPlayer == null || clip == null)
        {
            Debug.LogError($"Video Player or Clip missing on {gameObject.name}!");
            return;
        }

        if (isPlayingVideo) return; // Prevent multiple videos at once

        isPlayingVideo = true;

        // Disable sprite animator
        if (spriteAnimator != null)
            spriteAnimator.enabled = false;

        // Prepare and play video
        videoPlayer.clip = clip;
        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        // Switch sprite renderer to show video texture
        if (spriteRenderer != null && renderTexture != null)
        {
            // Create material for video display
            if (videoMaterial == null)
            {
                videoMaterial = new Material(Shader.Find("Unlit/Texture"));
            }
            videoMaterial.mainTexture = renderTexture;
            spriteRenderer.material = videoMaterial;
        }

        videoPlayer.Play();
        Debug.Log($"Playing video on {gameObject.name}");
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        isPlayingVideo = false;

        // Return to sprite material
        if (spriteRenderer != null)
        {
            spriteRenderer.material = null; // Reset to default sprite material
            spriteRenderer.sprite = originalSprite;
        }

        // Re-enable animator for idle
        if (spriteAnimator != null)
            spriteAnimator.enabled = true;

        Debug.Log($"Video finished on {gameObject.name}");
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }

        if (renderTexture != null)
            Destroy(renderTexture);

        if (videoMaterial != null)
            Destroy(videoMaterial);
    }
}
