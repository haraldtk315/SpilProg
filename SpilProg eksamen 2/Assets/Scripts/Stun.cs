using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStunReceiver2D : NetworkBehaviour
{
    private Rigidbody2D rb;

    private bool stunned;
    private Coroutine stunRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public bool IsStunned => stunned;

    [ServerRpc(RequireOwnership = false)]
    public void StunServerRpc(float stunTime)
    {
        StunClientRpc(stunTime);
    }

    [ClientRpc]
    private void StunClientRpc(float stunTime)
    {
        Stun(stunTime);
    }

    public void Stun(float stunTime)
    {
        if (stunRoutine != null)
            StopCoroutine(stunRoutine);

        stunRoutine = StartCoroutine(StunRoutine(stunTime));
    }

    private IEnumerator StunRoutine(float stunTime)
    {
        stunned = true;

        float timer = 0f;

        while (timer < stunTime)
        {
            timer += Time.deltaTime;

            rb.linearVelocity = Vector2.zero;

            yield return null;
        }

        stunned = false;
        stunRoutine = null;
    }
}