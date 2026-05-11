using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Enabled")]
    [SerializeField] private bool allowMovement = true;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string walkParameter = "IsWalking";

    [Header("Sprite Flip")]
    [SerializeField] private Transform visualRoot;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector3 originalVisualScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (visualRoot == null)
            visualRoot = transform;

        originalVisualScale = visualRoot.localScale;
    }

    void Update()
    {
        if (!allowMovement)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (animator != null)
                animator.SetBool(walkParameter, false);

            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }
    }

    void FixedUpdate()
    {
        if (!allowMovement)
            return;
//Vandret bevægelse
        float moveInput =
            Keyboard.current.aKey.isPressed ? -1 :
            Keyboard.current.dKey.isPressed ? 1 : 0;

        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (animator != null)
            animator.SetBool(walkParameter, Mathf.Abs(moveInput) > 0.01f);

        if (moveInput > 0)
        {
            visualRoot.localScale = new Vector3(
                Mathf.Abs(originalVisualScale.x),
                originalVisualScale.y,
                originalVisualScale.z
            );
        }
        else if (moveInput < 0)
        {
            visualRoot.localScale = new Vector3(
                -Mathf.Abs(originalVisualScale.x),
                originalVisualScale.y,
                originalVisualScale.z
            );
        }
    }
}