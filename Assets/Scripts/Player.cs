using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Health health; 
    [SerializeField] public float moveSpeed = 24f;
    [SerializeField] public float jumpForce = 55f;
    [SerializeField] public float acceleration = 200f;
    [SerializeField] public float deceleration = 160f;

    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;
    public float fallMultiplier = 2f;
    public float lowJumpMultiplier = 3f;
    public float fastFallForce = 30f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool isGrounded;

    private float moveInput;
    private bool jumpHeld;
    private bool fastFallHeld;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float staggerTimer;

    // bumped by the blueberry powerup
    [HideInInspector] public float speedMultiplier = 1f;
    [HideInInspector] public float difficultyMultiplier = 1f;

    public bool IsGrounded => isGrounded;
    public bool IsStaggered => staggerTimer > 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // staggered = no control, that's what makes getting hit actually cost you
        if (staggerTimer > 0f)
        {
            staggerTimer -= Time.deltaTime;
            moveInput = 0f;
            jumpHeld = false;
            fastFallHeld = false;
            return;
        }

        ReadKeys();

        coyoteTimer = isGrounded ? coyoteTime : coyoteTimer - Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // while staggered, leave the knockback velocity alone - running Move() here
        // kills it almost instantly and you never see him get pushed
        if (staggerTimer > 0f)
        {
            if (rb.linearVelocity.y < 0f)
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
            return;
        }

        Move();
        Jump();
        ExtraGravity();
    }

    private void ReadKeys()
    {
        if (Keyboard.current == null) return;

        moveInput = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput += 1f;

        if (Keyboard.current.spaceKey.wasPressedThisFrame
            || Keyboard.current.upArrowKey.wasPressedThisFrame
            || Keyboard.current.wKey.wasPressedThisFrame)
            jumpBufferTimer = jumpBufferTime;

        jumpHeld = Keyboard.current.spaceKey.isPressed
            || Keyboard.current.upArrowKey.isPressed
            || Keyboard.current.wKey.isPressed;

        fastFallHeld = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;
    }

    private void Move()
    {
        float target = moveInput * moveSpeed * speedMultiplier * difficultyMultiplier;
        float rate = Mathf.Abs(target) > 0.01f ? acceleration : deceleration;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, target, rate * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);

        if (sprite != null && Mathf.Abs(moveInput) > 0.01f)
            sprite.flipX = moveInput < 0f;
    }

    private void Jump()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }

    private void ExtraGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;

            if (fastFallHeld)
                rb.AddForce(Vector2.down * fastFallForce);
        }
        else if (rb.linearVelocity.y > 0f && !jumpHeld)
        {
            // let go early and you get a shorter hop
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    public void Stagger(float duration, Vector2 knockback)
    {
        staggerTimer = duration;
        rb.linearVelocity = knockback;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
