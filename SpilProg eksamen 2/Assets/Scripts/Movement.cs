using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : NetworkBehaviour
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

    [Header("Visual Flip")]
    [SerializeField] private Transform visualRoot;

    private Rigidbody2D rb;
    private Vector3 originalVisualScale;
    private bool movementBlocked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        originalVisualScale = visualRoot != null ? visualRoot.localScale : Vector3.one;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (!allowMovement || movementBlocked)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (animator != null)
                animator.SetBool(walkParameter, false);

            return;
        }

        bool isGrounded = true;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        if (!allowMovement || movementBlocked)
            return;

        float moveInput =
            Keyboard.current.aKey.isPressed ? -1 :
            Keyboard.current.dKey.isPressed ? 1 : 0;

        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        bool isWalking = Mathf.Abs(moveInput) > 0.01f;

        if (animator != null)
            animator.SetBool(walkParameter, isWalking);

        if (moveInput > 0)
            FaceDirection(1f);
        else if (moveInput < 0)
            FaceDirection(-1f);
    }

    public void SetMovementBlocked(bool blocked)
    {
        movementBlocked = blocked;

        if (blocked && rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private void FaceDirection(float direction)
    {
        if (visualRoot == null)
            return;

        visualRoot.localScale = new Vector3(
            Mathf.Abs(originalVisualScale.x) * direction,
            originalVisualScale.y,
            originalVisualScale.z
        );
    }
}