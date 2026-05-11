using Unity.Netcode;
using UnityEngine;

public class PlayerFrameUI : NetworkBehaviour
{
    [Header("Player Frames")]
    [SerializeField] private GameObject player1Frame;
    [SerializeField] private GameObject player2Frame;
    [SerializeField] private GameObject player3Frame;
    [SerializeField] private GameObject player4Frame;

    private void Start()
    {
        UpdateFrames();
    }

    public override void OnNetworkSpawn()
    {
        UpdateFrames();

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientChanged;
        }
    }

    private void OnClientChanged(ulong clientId)
    {
        UpdateFrames();
    }

    private void UpdateFrames()
    {
        int playerCount = 0;

        if (NetworkManager.Singleton != null)
        {
            playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        }

        if (player1Frame != null)
            player1Frame.SetActive(playerCount >= 1);

        if (player2Frame != null)
            player2Frame.SetActive(playerCount >= 2);

        if (player3Frame != null)
            player3Frame.SetActive(playerCount >= 3);

        if (player4Frame != null)
            player4Frame.SetActive(playerCount >= 4);
    }
}