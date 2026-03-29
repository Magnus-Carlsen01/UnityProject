using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
     private Rigidbody2D rb;
    //Player mặc định quay về phía bên phải
    private bool isFacingRight = true;
    public ParticleSystem smokeFX;
    private BoxCollider2D playerCollider;
    public AudioManager audioManager;
    private Animator animator;
    private InputAction playerInputActions;

    //Biến "di chuyển"
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMove;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    bool isDashing = false;
    bool canDash = true;
    TrailRenderer trailRenderer;

    //Biến "nhảy"
    [Header("Jumping")]
   [SerializeField] private float jumpForce = 10f;
   private int maxJumps=2;
   int jumpsRemaining;

//Kiểm tra va chạm với mặt đất
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.01f);
    [SerializeField] private LayerMask groundLayer;
       bool isGrounded;
    bool isOnPlatform;

    //Kiểm tra va chạm với tường
    [Header("Wall Check")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 0.01f);
    [SerializeField] private LayerMask wallLayer;

    [Header("Gravity")]
    [SerializeField] private float baseGravity = 2f;
    [SerializeField] private float maxGravity = 10f;
    [SerializeField] private float fallSpeedMultiplier = 2f;

    [Header("WallMovement")]
    [SerializeField] private float wallSlideSpeed = 2f;
    private bool isWallSliding;
    //WallJump
    [Header("WallJump")]
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    Vector2 wallJumpForce = new Vector2(5f,10f);


    void Start()
    {
        //Tham chiếu đến UnityEngine
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
        audioManager = Object.FindFirstObjectByType<AudioManager>();
        animator = GetComponent<Animator>();
    }

    // Update từng khung hình
    void Update()
    {
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);
        if (isDashing)
       {
           return; // Nếu đang dashing, không thực hiện các hành động khác
        }
        OnTheGround();
       OnTheWall();
       Gravity();
       WallSlide();
       WallJump();
       
       if (!isWallJumping)
       {
        rb.linearVelocity = new Vector2(horizontalMove*moveSpeed, rb.linearVelocity.y);
        Flip();
      }
       
    }
    
    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && isOnPlatform && playerCollider.enabled)
        {
            StartCoroutine(DisablePlayerCollider(0.5f));
        }
    }
    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        playerCollider.enabled = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
           isOnPlatform = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
           isOnPlatform = false;
        }
    }
    //Nhảy, nhảy 2 lần
    public void Jump(InputAction.CallbackContext context)
    {
        if(jumpsRemaining>0)
        {
            if (context.performed)
            {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsRemaining--;

            JumpFX();
        }
        else if (context.canceled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y*0.5f);
            jumpsRemaining--;

            JumpFX();
        }
        }
        if (context.performed && wallJumpTimer>0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpForce.x, wallJumpForce.y);
            wallJumpTimer = 0;

            JumpFX();

            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        //scaler.y *= -1;
        transform.localScale = scaler;      
            }
            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        } 
    }

    private void JumpFX()
    {
        smokeFX.Play();
        animator.SetTrigger("Jump");
        audioManager.PlayJumpSound();
    }
    //Trượt trên tường
    private void WallSlide()
    {
        if (!isGrounded && OnTheWall() && horizontalMove != 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }
    
    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }
    private void CancelWallJump()
    {
        isWallJumping = false;
    }
    //Tính toán trọng lực khi rơi
    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity*fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxGravity));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }   
     //Lật nhân vật
    void Flip()
    {
        if (horizontalMove > 0.01f && !isFacingRight||horizontalMove < -0.01f && isFacingRight)
        {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        //scaler.y *= -1;
        transform.localScale = scaler;
        if (rb.linearVelocity.y == 0)
        {
            smokeFX.Play();
        }
        }
    }
    //Di chuyển nhân vật
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMove= context.ReadValue<Vector2>().x;
    }
    //Dashing
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }
    private IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        isDashing = true;
        canDash = false;
        trailRenderer.emitting = true;
        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        isDashing = false;
        trailRenderer.emitting = false;

        Physics2D.IgnoreLayerCollision(7, 8, false);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    //Kiểm tra va chạm với mặt đất
    private void OnTheGround()
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    //Kiểm tra va chạm với tường
    private bool OnTheWall()
    {
        return (Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, wallLayer));
    }
    //Vẽ khung kiểm tra va chạm với mặt đất và tường
    private void OnDrawGizmosSelected()
    {
        //Vẽ khung kiểm tra va chạm với mặt đất
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        //Vẽ khung kiểm tra va chạm với tường
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
   
}
