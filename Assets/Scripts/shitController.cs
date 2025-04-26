using UnityEngine;

public class ShitManager : MonoBehaviour
{
    private Animator animator;
    private bool isCleaning = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        if (!isCleaning)
        {
            isCleaning = true;
            animator.SetTrigger("clean"); // Ending animasyonu için trigger kullanýyoruz
            Invoke("DestroyShit", 0.4f); // End animasyonunun süresi kadar bekle
        }
    }

    void DestroyShit()
    {
        Destroy(gameObject);
    }
}
