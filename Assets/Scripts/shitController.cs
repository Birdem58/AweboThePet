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
            animator.SetTrigger("clean"); // Ending animasyonu i�in trigger kullan�yoruz
            Invoke("DestroyShit", 0.4f); // End animasyonunun s�resi kadar bekle
        }
    }

    void DestroyShit()
    {
        Destroy(gameObject);
    }
}
