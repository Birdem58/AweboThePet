using UnityEngine;

public class ShitController : MonoBehaviour
{
    private Animator animator;
    private bool isCleaning = false;
    private float spawnTime;
    public  bool penaltyApplied = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        PetManager.Instance.RegisterPoop(this);
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

    void Update()
    {
        // 5 saniyeyi geçerse ceza uygula (sadece 1 kez)
        if (Time.time - spawnTime > 5f && !penaltyApplied)
        {
            PetManager.Instance.AddHygienePenalty();
            penaltyApplied = true;
        }
    }

    void DestroyShit()
    {
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        PetManager.Instance.RemovePoop(this); // PetManager'dan kaldýr
    }
}
