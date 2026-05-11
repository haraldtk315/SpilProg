using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRagdollReceiver2D : NetworkBehaviour
{
    [Header("Movement Script")]
    [SerializeField] private MonoBehaviour movementScript;

    [Header("Ragdoll Settings")]
    [SerializeField] private float ragdollTime = 1.5f;
    [SerializeField] private float hitForce = 20f;
    [SerializeField] private float upwardForce = 10f;
    [SerializeField] private float spinForce = 600f;

    private Rigidbody2D rb;
    private RigidbodyConstraints2D originalConstraints;
    private Coroutine ragdollRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalConstraints = rb.constraints;

        if (movementScript == null)
            movementScript = GetComponent<Movement>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RagdollServerRpc(Vector2 hitDirection)
    {
        RagdollClientRpc(hitDirection);
    }

    [ClientRpc]
    private void RagdollClientRpc(Vector2 hitDirection)
    {
        StartRagdoll(hitDirection);
    }

    public void StartRagdoll(Vector2 hitDirection)
    {
        if (ragdollRoutine != null)
            StopCoroutine(ragdollRoutine);

        ragdollRoutine = StartCoroutine(RagdollRoutine(hitDirection));
    }

    private IEnumerator RagdollRoutine(Vector2 hitDirection)
    {
        if (movementScript != null)
            movementScript.enabled = false;

        rb.constraints = RigidbodyConstraints2D.None;
        rb.freezeRotation = false;

        hitDirection.y += upwardForce;
        hitDirection.Normalize();

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.AddForce(hitDirection * hitForce, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-spinForce, spinForce), ForceMode2D.Impulse);

        yield return new WaitForSeconds(ragdollTime);

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;

        rb.constraints = originalConstraints;

        if (movementScript != null)
            movementScript.enabled = true;

        ragdollRoutine = null;
    }
}