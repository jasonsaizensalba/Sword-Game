using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    public Animator mAnimator;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }



    public void Katana()
    {

        mAnimator.SetTrigger("Katana");
        Debug.Log("KatanaSlash animation played.");

    }
    public void Rapier()
    {

        mAnimator.SetTrigger("Rapier");
        Debug.Log("RapierSlash animation played.");

    }

    public void Claymore()
    {

        mAnimator.SetTrigger("Claymore");
        Debug.Log("ClaymoreSlash animation played.");

    }
    


}
