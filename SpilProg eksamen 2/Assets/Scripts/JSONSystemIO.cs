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
        SaveData data = new SaveData();
        data.score1 = ScoreBoardManager.scores[0].score;
        data.score2 = ScoreBoardManager.scores[1].score;

        string json = JsonUtility.ToJson(data, true);
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
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        ScoreBoardManager.scores[0] = new PlayerScore { score = data.score1 };
        ScoreBoardManager.scores[1] = new PlayerScore { score = data.score2 };

    }

    [System.Serializable]
    public class SaveData
    {
        public int score1;
        public int score2;
    }
}
