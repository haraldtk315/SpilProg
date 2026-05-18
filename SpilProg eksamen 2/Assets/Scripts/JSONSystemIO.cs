using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class JSONSystemIO : MonoBehaviour
{
    private string filePath;
    void Start()
    {
        filePath = Application.persistentDataPath + "/playerdata.json";
        Debug.Log("Save path: " + filePath);
    }

    public void SaveScore()
    {
        string json = JsonUtility.ToJson(ScoreBoardManager.scores[0].score, true);
         json += JsonUtility.ToJson(ScoreBoardManager.scores[1].score, true);

        File.WriteAllText(filePath, json);
        Debug.Log("Score saved to: " + filePath);
        Debug.Log(json);

        Debug.Log("Score gemt: " + ScoreBoardManager.scores);
    }

    public void LoadScore()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file does not exist: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        ScoreBoardManager.scores[0] = JsonUtility.FromJson<PlayerScore>(json);
        ScoreBoardManager.scores[1] = JsonUtility.FromJson<PlayerScore>(json);

    }
}
