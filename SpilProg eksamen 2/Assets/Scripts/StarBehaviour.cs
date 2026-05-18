using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class StarBehaviour : NetworkBehaviour
{
    public StarManager starManager;

    private void Awake()
    {
        starManager = FindFirstObjectByType<StarManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;
        ScoreBoardManager.instance.AddScoreServerRpc();
        starManager.GrabStarServerRpc();
    }
    
}
