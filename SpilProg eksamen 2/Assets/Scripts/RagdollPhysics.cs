using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRagdollReceiver2D : NetworkBehaviour
{
    [Header("Movement Script")]
    [SerializeField] private Movement movementScript;

    [Header("Visual Root")]
    [SerializeField] private Transform visualRoot;

    [Header("Ragdoll")]
    [SerializeField] private float ragdollTime = 2f;

    [Header("Push")]
    [SerializeField] private float sideVelocity = 22f;
    [SerializeField] private float verticalVelocity = 0.5f;
    [SerializeField] private float pushBurstTime = 0.18f;

    [Header("Spin")]
    [SerializeField] private float spinSpeed = 450f;

    [Header("Border Wrap")]
    [SerializeField] private bool useBorderWrapping = true;

    private Rigidbody2D rb;
    private Coroutine ragdollRoutine;
    private Quaternion originalVisualRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (movementScript == null)
            movementScript = GetComponent<Movement>();

        if (visualRoot != null)
            originalVisualRotation = visualRoot.localRotation;

        FreezeRotation();
    }

    private void FixedUpdate()
    {
        if (!useBorderWrapping)
            return;

        if (BorderManager.instance == null)
            return;

        WrapInsideBorder();
    }

    [ClientRpc]
    public void PlayRagdollClientRpc(float sideDirection)
    {
        if (ragdollRoutine != null)
            StopCoroutine(ragdollRoutine);

        ragdollRoutine = StartCoroutine(RagdollRoutine(sideDirection));
    }

    private IEnumerator RagdollRoutine(float sideDirection)
    {
        if (movementScript != null)
            movementScript.SetMovementBlocked(true);

        UnfreezeRotation();

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        float timer = 0f;

        while (timer < ragdollTime)
        {
            timer += Time.deltaTime;

            if (timer < pushBurstTime)
            {
                rb.linearVelocity = new Vector2(
                    sideDirection * sideVelocity,
                    verticalVelocity
                );
            }

            rb.angularVelocity = -sideDirection * spinSpeed;

            if (!IsOwner && visualRoot != null)
            {
                visualRoot.Rotate(
                    0f,
                    0f,
                    -sideDirection * spinSpeed * Time.deltaTime
                );
            }

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.rotation = 0f;
        transform.rotation = Quaternion.identity;

        if (visualRoot != null)
            visualRoot.localRotation = originalVisualRotation;

        FreezeRotation();

        if (movementScript != null)
            movementScript.SetMovementBlocked(false);

        ragdollRoutine = null;
    }

    private void WrapInsideBorder()
    {
        Vector2 pos = rb.position;
        Vector2 newPos = pos;
        bool shouldWrap = false;

        if (pos.x < BorderManager.instance.leftLimit)
        {
            newPos.x = BorderManager.instance.rightLimit;
            shouldWrap = true;
        }
        else if (pos.x > BorderManager.instance.rightLimit)
        {
            newPos.x = BorderManager.instance.leftLimit;
            shouldWrap = true;
        }

        if (pos.y < BorderManager.instance.bottomLimit)
        {
            newPos.y = BorderManager.instance.topLimit;
            shouldWrap = true;
        }
        else if (pos.y > BorderManager.instance.topLimit)
        {
            newPos.y = BorderManager.instance.bottomLimit;
            shouldWrap = true;
        }

        if (!shouldWrap)
            return;

        Vector2 savedVelocity = rb.linearVelocity;
        float savedAngularVelocity = rb.angularVelocity;

        rb.position = newPos;
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

        rb.linearVelocity = savedVelocity;
        rb.angularVelocity = savedAngularVelocity;
    }

    private void FreezeRotation()
    {
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
        rb.freezeRotation = true;
    }

    private void UnfreezeRotation()
    {
        rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
        rb.freezeRotation = false;
    }
}