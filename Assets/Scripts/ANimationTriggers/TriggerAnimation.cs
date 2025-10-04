using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator; // Assign in Inspector
    [SerializeField] private Animator Rapier; // Assign in Inspector

    public void PlayKatanaSlash()
    {
        if (animator != null)
            animator.SetTrigger("Katana");
    }
     public void PlayRapierSlash()
    {
        if (animator != null)
            animator.SetTrigger("Rapier");
    }
}
