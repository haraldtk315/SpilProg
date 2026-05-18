using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class MultiplayerMenu : MonoBehaviour
{
    [Header("Menu UI")]
    [SerializeField] private GameObject menuRoot;

    [Header("Join UI")]
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private GameObject joinServerButton;

    [Header("Host UI")]
    [SerializeField] private GameObject hostCodePanel;
    [SerializeField] private TMP_Text hostCodeText;

    [Header("Code UI")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_InputField joinCodeInputField;

    [Header("Connection")]
    [SerializeField] private ushort gamePort = 7777;
    [SerializeField] private int discoveryPort = 47777;
    [SerializeField] private float searchTime = 8f;

    private string hostCode;
    private string hostLocalIp;

    private UdpClient broadcastClient;
    private Coroutine broadcastCoroutine;
    private Coroutine searchCoroutine;

    private const string DISCOVERY_PREFIX = "LOCAL_GAME_CODE";

    private void Start()
    {
        HideJoinUI();
        HideHostCodeUI();
    }

    public void HostGame()
    {
        HideJoinUI();
        ShowHostCodeUI();

        if (NetworkManager.Singleton == null)
        {
            SetStatus("No NetworkManager found.");
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport == null)
        {
            SetStatus("No UnityTransport found.");
            return;
        }

        hostCode = GenerateHostCode();
        hostLocalIp = GetLocalIPAddress();

        if (string.IsNullOrEmpty(hostLocalIp))
        {
            SetStatus("Could not find local IP.");
            return;
        }

        if (hostCodeText != null)
            hostCodeText.text = hostCode;

        transport.SetConnectionData("0.0.0.0", gamePort);

        bool started = NetworkManager.Singleton.StartHost();

        if (!started)
        {
            SetStatus("Host failed to start.");
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        StartBroadcasting();

        SetStatus("Hosting | Code: " + hostCode + " | IP: " + hostLocalIp);

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= 2)
        {
            HideMenu();
        }
    }

    public void OpenJoinPanel()
    {
        HideHostCodeUI();
        ShowJoinUI();

        if (joinCodeInputField != null)
            joinCodeInputField.text = "";

        SetStatus("Enter host code.");
    }

    public void CloseJoinPanel()
    {
        HideJoinUI();
    }

    public void JoinGame()
    {
        HideHostCodeUI();

        if (joinCodeInputField == null)
        {
            SetStatus("Join input missing.");
            return;
        }

        string code = joinCodeInputField.text.Trim();

        if (code.Length < 6 || code.Length > 9)
        {
            SetStatus("Code must be 6-9 numbers.");
            return;
        }

        if (searchCoroutine != null)
            StopCoroutine(searchCoroutine);

        searchCoroutine = StartCoroutine(SearchForServer(code));
    }

    private IEnumerator SearchForServer(string wantedCode)
    {
        SetStatus("Searching for local server...");

        UdpClient listener = null;

        try
        {
            listener = new UdpClient();

            listener.Client.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReuseAddress,
                true
            );

            listener.Client.Bind(new IPEndPoint(IPAddress.Any, discoveryPort));
            listener.EnableBroadcast = true;
        }
        catch (System.Exception e)
        {
            SetStatus("Could not listen. " + e.Message);
            yield break;
        }

        float timer = 0f;

        while (timer < searchTime)
        {
            timer += Time.deltaTime;

            while (listener.Available > 0)
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, discoveryPort);

                byte[] data = listener.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);

                Debug.Log("Discovery message received: " + message);

                string[] parts = message.Split('|');

                if (parts.Length != 4)
                    continue;

                string prefix = parts[0];
                string code = parts[1];
                string hostIp = parts[2];
                string portText = parts[3];

                if (prefix != DISCOVERY_PREFIX)
                    continue;

                if (code != wantedCode)
                    continue;

                if (!ushort.TryParse(portText, out ushort foundPort))
                    continue;

                listener.Close();

                ConnectToHost(hostIp, foundPort);
                yield break;
            }

            yield return null;
        }

        listener.Close();
        SetStatus("No server found.");
    }

    private void ConnectToHost(string hostIp, ushort port)
    {
        if (NetworkManager.Singleton == null)
        {
            SetStatus("No NetworkManager found.");
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport == null)
        {
            SetStatus("No UnityTransport found.");
            return;
        }

        transport.SetConnectionData(hostIp, port);

        bool started = NetworkManager.Singleton.StartClient();

        if (!started)
        {
            SetStatus("Join failed.");
            return;
        }

        SetStatus("Joining " + hostIp + "...");
        HideMenu();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton == null)
            return;

        if (!NetworkManager.Singleton.IsHost)
            return;

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= 2)
        {
            HideMenu();
        }
    }

    private void StartBroadcasting()
    {
        StopBroadcasting();

        broadcastClient = new UdpClient();
        broadcastClient.EnableBroadcast = true;

        broadcastCoroutine = StartCoroutine(BroadcastRoutine());
    }

    private IEnumerator BroadcastRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        while (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            string message =
                DISCOVERY_PREFIX + "|" +
                hostCode + "|" +
                hostLocalIp + "|" +
                gamePort;

            byte[] data = Encoding.UTF8.GetBytes(message);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, discoveryPort);
            broadcastClient.Send(data, data.Length, endPoint);

            Debug.Log("Broadcasting: " + message);

            yield return wait;
        }
    }

    private void StopBroadcasting()
    {
        if (broadcastCoroutine != null)
        {
            StopCoroutine(broadcastCoroutine);
            broadcastCoroutine = null;
        }

        if (broadcastClient != null)
        {
            broadcastClient.Close();
            broadcastClient = null;
        }
    }

    public void StopGame()
    {
        StopBroadcasting();

        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
            searchCoroutine = null;
        }

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.Shutdown();
        }

        if (menuRoot != null)
            menuRoot.SetActive(true);

        HideJoinUI();
        HideHostCodeUI();

        SetStatus("Disconnected.");
    }

    private void ShowJoinUI()
    {
        if (joinPanel != null)
            joinPanel.SetActive(true);

        if (joinServerButton != null)
            joinServerButton.SetActive(true);
    }

    private void HideJoinUI()
    {
        if (joinPanel != null)
            joinPanel.SetActive(false);

        if (joinServerButton != null)
            joinServerButton.SetActive(false);
    }

    private void ShowHostCodeUI()
    {
        if (hostCodePanel != null)
            hostCodePanel.SetActive(true);
    }

    private void HideHostCodeUI()
    {
        if (hostCodePanel != null)
            hostCodePanel.SetActive(false);
    }

    private string GenerateHostCode()
    {
        int length = Random.Range(6, 10);
        string code = "";

        for (int i = 0; i < length; i++)
            code += Random.Range(0, 10).ToString();

        return code;
    }

    private string GetLocalIPAddress()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily != AddressFamily.InterNetwork)
                continue;

            string ipText = ip.ToString();

            if (ipText.StartsWith("192.168.") ||
                ipText.StartsWith("10.") ||
                ipText.StartsWith("172.16.") ||
                ipText.StartsWith("172.17.") ||
                ipText.StartsWith("172.18.") ||
                ipText.StartsWith("172.19.") ||
                ipText.StartsWith("172.20.") ||
                ipText.StartsWith("172.21.") ||
                ipText.StartsWith("172.22.") ||
                ipText.StartsWith("172.23.") ||
                ipText.StartsWith("172.24.") ||
                ipText.StartsWith("172.25.") ||
                ipText.StartsWith("172.26.") ||
                ipText.StartsWith("172.27.") ||
                ipText.StartsWith("172.28.") ||
                ipText.StartsWith("172.29.") ||
                ipText.StartsWith("172.30.") ||
                ipText.StartsWith("172.31."))
            {
                return ipText;
            }
        }

        return "";
    }

    private void HideMenu()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    private void SetStatus(string message)
    {
        Debug.Log(message);

        if (statusText != null)
            statusText.text = message;
    }

    private void OnDestroy()
    {
        StopBroadcasting();

        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }
}