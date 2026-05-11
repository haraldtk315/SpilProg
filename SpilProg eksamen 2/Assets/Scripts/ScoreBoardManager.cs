using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreBoardManager : NetworkBehaviour
{
    public static ScoreBoardManager instance;

    public NetworkList<int> playerScores = new NetworkList<int>();

    [SerializeField] private JSONSystemIO jsonSystem;

    [Header("Score UI")]
    [SerializeField] private TMP_Text[] scoreTexts = new TMP_Text[4];

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            EnsureScoreSlotsExist();
        }
    }

    private void Update()
    {
        UpdateScoreUI();
    }

    private void EnsureScoreSlotsExist()
    {
        while (playerScores.Count < 4)
        {
            playerScores.Add(0);
        }
    }

    private void UpdateScoreUI()
    {
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (scoreTexts[i] == null)
                continue;

            if (i < playerScores.Count)
            {
                scoreTexts[i].text = playerScores[i].ToString();
            }
            else
            {
                scoreTexts[i].text = "0";
            }
        }
    }

    public void AddScoreForPlayer(ulong clientId, int amount = 1)
    {
        if (!IsServer)
            return;

        EnsureScoreSlotsExist();

        int playerIndex = GetPlayerIndex(clientId);

        if (playerIndex < 0 || playerIndex >= playerScores.Count)
            return;

        playerScores[playerIndex] += amount;
    }

    private int GetPlayerIndex(ulong clientId)
    {
        int index = 0;

        foreach (ulong connectedClientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (connectedClientId == clientId)
                return index;

            index++;
        }

        return -1;
    }

    public void SaveCurrentScore()
    {
        if (jsonSystem == null)
            return;

        int totalScore = 0;

        for (int i = 0; i < playerScores.Count; i++)
        {
            totalScore += playerScores[i];
        }

        jsonSystem.SaveScore(totalScore);
    }

    public void LoadSavedScore()
    {
        if (!IsServer)
            return;

        if (jsonSystem == null)
            return;

        EnsureScoreSlotsExist();

        int loadedScore = jsonSystem.LoadScore();

        playerScores[0] = loadedScore;
    }
}