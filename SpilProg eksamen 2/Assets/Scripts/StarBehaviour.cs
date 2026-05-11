using Unity.Netcode;
using UnityEngine;

public class StarBehaviour : NetworkBehaviour
{
    public StarManager starManager;

    [SerializeField] private string[] playerTags =
    {
        "Player1",
        "Player2",
        "Player3",
        "Player4"
    };

    private bool collected = false;

    private void Awake()
    {
        starManager = FindFirstObjectByType<StarManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        if (collected)
            return;

        if (!IsPlayer(collision))
            return;

        NetworkObject playerNetworkObject =
            collision.GetComponentInParent<NetworkObject>();

        if (playerNetworkObject == null)
            return;

        collected = true;

        if (ScoreBoardManager.instance != null)
        {
            ScoreBoardManager.instance.AddScoreForPlayer(
                playerNetworkObject.OwnerClientId,
                1
            );
        }

        if (starManager != null)
        {
            starManager.StartCoroutine(starManager.DestroyStar());
        }
    }

    private bool IsPlayer(Collider2D collision)
    {
        foreach (string playerTag in playerTags)
        {
            if (collision.CompareTag(playerTag))
                return true;
        }

        return false;
    }
}