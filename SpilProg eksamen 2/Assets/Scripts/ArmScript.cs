using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsArmSwing2D : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private Key attackKey = Key.E;

    [Header("Pivot Setup")]
    [SerializeField] private Transform armPivot;

    [Header("Arm Hit Trigger")]
    [SerializeField] private Collider2D armTriggerCollider;

    [Header("Dummy Arm Movement")]
    [SerializeField] private bool useDummyIdleMovement = true;
    [SerializeField] private float idleWobbleAmount = 12f;
    [SerializeField] private float idleWobbleSpeed = 4f;
    [SerializeField] private float returnSpeed = 8f;

    [Header("Swing")]
    [SerializeField] private float swingDuration = 0.45f;
    [SerializeField] private float swingDegrees = 360f;
    [SerializeField] private AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Player Tags")]
    [SerializeField] private string[] playerTags =
    {
        "Player1",
        "Player2",
        "Player3",
        "Player4"
    };

    [Header("Hit")]
    [SerializeField] private float hitCooldown = 0.5f;

    private bool isSwinging;
    private float lastHitTime = -999f;
    private Quaternion startRotation;
    private NetworkObject ownerNetworkObject;

    private void Awake()
    {
        ownerNetworkObject = GetComponentInParent<NetworkObject>();

        if (armPivot == null)
            armPivot = transform;

        if (armTriggerCollider == null)
            armTriggerCollider = GetComponentInChildren<Collider2D>();

        if (armTriggerCollider != null)
        {
            armTriggerCollider.isTrigger = true;
            armTriggerCollider.enabled = false;
        }

        startRotation = armPivot.localRotation;
    }

    private void Update()
    {
        if (!IsMyArm())
            return;

        if (Keyboard.current != null && Keyboard.current[attackKey].wasPressedThisFrame)
        {
            SwingServerRpc();
        }
    }

    private void LateUpdate()
    {
        if (isSwinging || armPivot == null)
            return;

        float wobble = useDummyIdleMovement
            ? Mathf.Sin(Time.time * idleWobbleSpeed) * idleWobbleAmount
            : 0f;

        Quaternion targetRotation =
            startRotation * Quaternion.Euler(0f, 0f, wobble);

        armPivot.localRotation = Quaternion.Lerp(
            armPivot.localRotation,
            targetRotation,
            returnSpeed * Time.deltaTime
        );
    }

    [ServerRpc(RequireOwnership = false)]
    private void SwingServerRpc()
    {
        SwingClientRpc();
    }

    [ClientRpc]
    private void SwingClientRpc()
    {
        StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        if (isSwinging)
            yield break;

        isSwinging = true;

        if (armTriggerCollider != null)
            armTriggerCollider.enabled = true;

        float timer = 0f;
        float startZ = armPivot.localEulerAngles.z;

        while (timer < swingDuration)
        {
            timer += Time.deltaTime;

            float t = timer / swingDuration;
            float curvedT = swingCurve.Evaluate(t);

            float zRotation = startZ + swingDegrees * curvedT;
            armPivot.localRotation = Quaternion.Euler(0f, 0f, zRotation);

            yield return null;
        }

        if (armTriggerCollider != null)
            armTriggerCollider.enabled = false;

        armPivot.localRotation = startRotation;
        isSwinging = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsMyArm())
            return;

        if (!isSwinging)
            return;

        if (Time.time < lastHitTime + hitCooldown)
            return;

        if (!HasValidPlayerTag(other))
            return;

        if (other.transform.root == transform.root)
            return;

        NetworkObject hitNetworkObject = other.GetComponentInParent<NetworkObject>();

        if (hitNetworkObject == null)
            return;

        lastHitTime = Time.time;

        float sideDirection =
            other.transform.position.x > transform.root.position.x ? 1f : -1f;

        ReportHitServerRpc(hitNetworkObject.NetworkObjectId, sideDirection);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReportHitServerRpc(ulong hitNetworkObjectId, float sideDirection)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(
                hitNetworkObjectId,
                out NetworkObject hitObject))
            return;

        PlayerRagdollReceiver2D ragdollReceiver =
            hitObject.GetComponent<PlayerRagdollReceiver2D>();

        if (ragdollReceiver == null)
            ragdollReceiver = hitObject.GetComponentInChildren<PlayerRagdollReceiver2D>();

        if (ragdollReceiver == null)
            return;

        ragdollReceiver.PlayRagdollClientRpc(sideDirection);
    }

    private bool HasValidPlayerTag(Collider2D other)
    {
        foreach (string tag in playerTags)
        {
            if (other.CompareTag(tag))
                return true;
        }

        return false;
    }

    private bool IsMyArm()
    {
        if (ownerNetworkObject == null)
            return true;

        if (!ownerNetworkObject.IsSpawned)
            return true;

        return ownerNetworkObject.IsOwner;
    }
}