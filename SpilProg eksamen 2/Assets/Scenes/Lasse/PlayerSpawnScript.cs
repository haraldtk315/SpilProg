using Unity.Netcode;
using UnityEngine;

public class MultiplayerPlayerSpawner : NetworkBehaviour
{
    [Header("Player Prefabs")]
    [SerializeField] private GameObject[] playerPrefabs;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    private int spawnedPlayers = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayerForClient;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayerForClient(clientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayerForClient;
    }

    private void SpawnPlayerForClient(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null)
            return;

        int index = spawnedPlayers;

        if (index >= playerPrefabs.Length)
            index = playerPrefabs.Length - 1;

        Transform spawnPoint = null;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int spawnIndex = Mathf.Clamp(index, 0, spawnPoints.Length - 1);
            spawnPoint = spawnPoints[spawnIndex];
        }

        Vector3 spawnPosition = spawnPoint != null
            ? new Vector3(
                spawnPoint.position.x,
                spawnPoint.position.y,
                spawnPoint.position.z
            )
            : Vector3.zero;

        Quaternion spawnRotation = spawnPoint != null
            ? spawnPoint.rotation
            : Quaternion.identity;

        GameObject player = Instantiate(
            playerPrefabs[index],
            spawnPosition,
            spawnRotation
        );

        player.transform.position = spawnPosition;
        player.transform.rotation = spawnRotation;

        NetworkObject networkObject = player.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId, true);

        player.transform.position = spawnPosition;
        player.transform.rotation = spawnRotation;

        spawnedPlayers++;
    }
}