using UnityEngine;

public class Openinganim : MonoBehaviour
{
    private Animator animator;
    private bool hasPlayed = false;
    public GameObject canvas;
    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null && !hasPlayed)
        {
            animator.Play("AnimationName"); // Animasyon adýný doðru yaz
            hasPlayed = true;
            StartCoroutine(DisableAnimatorWhenDone());
        }
    }

    private System.Collections.IEnumerator DisableAnimatorWhenDone()
    {
        // Animasyonun uzunluðunu al
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(animationLength);
        canvas.SetActive(true); // Canvas'ý aç

        animator.enabled = false; // Ýstersen animator'ý tamamen kapatabilirsin
        
    }
}
