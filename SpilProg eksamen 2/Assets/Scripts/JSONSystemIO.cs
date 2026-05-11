using UnityEngine;
using System.IO;

public class JSONSystemIO : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        filePath = Application.persistentDataPath + "/playerdata.json";
    }

    public void SaveScore(int score)
    {
        ScoreData data = new ScoreData();
        data.score = score;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);

        Debug.Log("Score gemt: " + score);
    }

    public int LoadScore()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);

            Debug.Log("Score loaded: " + data.score);
            return data.score;
        }

        Debug.LogWarning("Ingen save-fil fundet");
        return 0;
    }
}

[System.Serializable]
public class ScoreData
{
    public int score;
}