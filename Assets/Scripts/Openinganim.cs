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
            animator.Play("AnimationName"); // Animasyon ad�n� do�ru yaz
            hasPlayed = true;
            StartCoroutine(DisableAnimatorWhenDone());
        }
    }

    private System.Collections.IEnumerator DisableAnimatorWhenDone()
    {
        // Animasyonun uzunlu�unu al
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(animationLength);
        canvas.SetActive(true); // Canvas'� a�

        animator.enabled = false; // �stersen animator'� tamamen kapatabilirsin
        
    }
}
