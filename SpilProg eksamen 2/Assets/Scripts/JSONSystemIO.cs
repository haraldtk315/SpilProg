using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class JSONSystemIO : MonoBehaviour
{
    private string filePath;

    public TMPro.TMP_Text scoreText;
    private int score;

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

    public void LoadScore()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file does not exist: " + filePath);
            scoreText.text = "0";
        }

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);
            score = data.score;
            scoreText.text = score.ToString();

            Debug.Log("Score loaded: " + data.score);

        } 
    }
}

[System.Serializable]
public class ScoreData
{
    public int score;
}