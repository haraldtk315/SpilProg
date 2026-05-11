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

    [Header("Ragdoll Hit")]
    [SerializeField] private float hitCooldown = 0.5f;

    private bool isSwinging;
    private float lastHitTime = -999f;
    private Quaternion startRotation;

    private void Awake()
    {
        if (armPivot == null)
            armPivot = transform;

        startRotation = armPivot.localRotation;
    }

    private void Update()
    {
        if (IsSpawned && !IsOwner)
            return;

        if (Keyboard.current != null && Keyboard.current[attackKey].wasPressedThisFrame)
        {
            if (IsSpawned)
                SwingServerRpc();
            else
                StartCoroutine(SwingRoutine());
        }
    }

    private void LateUpdate()
    {
        if (isSwinging)
            return;

        if (armPivot == null)
            return;

        if (!useDummyIdleMovement)
        {
            armPivot.localRotation = Quaternion.Lerp(
                armPivot.localRotation,
                startRotation,
                returnSpeed * Time.deltaTime
            );

            return;
        }

        float wobble = Mathf.Sin(Time.time * idleWobbleSpeed) * idleWobbleAmount;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, 0f, wobble);

        armPivot.localRotation = Quaternion.Lerp(
            armPivot.localRotation,
            targetRotation,
            returnSpeed * Time.deltaTime
        );
    }

    [ServerRpc]
    private void SwingServerRpc()
    {
        if (isSwinging)
            return;

        StartCoroutine(SwingRoutine());
        SwingClientRpc();
    }

    [ClientRpc]
    private void SwingClientRpc()
    {
        if (IsServer)
            return;

        StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        if (isSwinging)
            yield break;

        isSwinging = true;

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

        armPivot.localRotation = startRotation;
        isSwinging = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isSwinging)
            return;

        if (Time.time < lastHitTime + hitCooldown)
            return;

        if (!HasValidPlayerTag(other))
            return;

        if (other.transform.root == transform.root)
            return;

        PlayerRagdollReceiver2D ragdollReceiver =
            other.GetComponentInParent<PlayerRagdollReceiver2D>();

        if (ragdollReceiver == null)
            return;

        lastHitTime = Time.time;

        Vector2 hitDirection =
            other.transform.position - transform.position;

        if (IsSpawned)
            ragdollReceiver.RagdollServerRpc(hitDirection);
        else
            ragdollReceiver.StartRagdoll(hitDirection);
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
}