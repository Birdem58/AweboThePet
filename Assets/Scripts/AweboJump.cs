using UnityEngine;

public class AweboJump : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Yer kontrolü
        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            GetComponent<Animator>().SetBool("IsJumping", false);
        }
        else
        {
            GetComponent<Animator>().SetBool("IsJumping", true);
        }
    }
}