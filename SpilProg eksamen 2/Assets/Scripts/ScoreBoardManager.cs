using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ScoreBoardManager : NetworkBehaviour
{
    
    
    public static ScoreBoardManager instance;

    public static NetworkList<PlayerScore> scores = new NetworkList<PlayerScore>();

    [SerializeField] private JSONSystemIO jsonSystem;
    [SerializeField] List<TMPro.TMP_Text> scoreTexts;

    [ServerRpc]
    public void AddScoreServerRpc(ulong playerID)
    {
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i].player == playerID)
            {
                PlayerScore newScore = scores[i];
                newScore.score++;
                scores[i] = newScore;
                jsonSystem.SaveScore(scores[i].score);
            }
        }
        DataBaseManager.InsertScore(scores[0].score, scores[1].score, System.DateTime.Now.ToString(), "Star collected" );
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void AddPlayerScoreRpc(ulong playerID)
    {
        PlayerScore score = new PlayerScore();
        score.player = playerID;
        scores.Add(score);
    }

    public void Update()
    {
        int i = 0;
        foreach (PlayerScore score in scores)
        {
            scoreTexts[i].text = score.score.ToString();
            i++;
        }
    }
    public void SaveCurrentScore(int currentScore)
    {
        jsonSystem.SaveScore(currentScore);
    }

    public void LoadSavedScore()
    {
        if (IsServer)
        {
           jsonSystem.LoadScore();
        }
    }



}
