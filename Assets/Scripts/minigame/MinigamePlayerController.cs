using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private bool isGrounded;
    public Transform groundCheck; // Yere temas kontrol�
    public LayerMask groundLayer; // Hangi layer yerde say�lacak

    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = 0;

        float speed = Mathf.Abs(rb.velocity.x);

        animator.SetFloat("velocity", speed);

        // Yere bas�yor mu kontrol�
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Coyote Time y�netimi
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            coyoteTimeCounter = 0f;
        }

        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(2,2, 2); // Sa�a bak�yor
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-2, 2, 2); // Sola bak�yor
        }

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }
}
