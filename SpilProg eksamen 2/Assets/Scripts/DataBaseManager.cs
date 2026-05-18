using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using UnityEditor.MemoryProfiler;

public class DataBaseManager : MonoBehaviour
{
    
    //Name of the DB
    private static string dbName = "SpilProg2ExamDatabase";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateDB();
    }
    
    public void CreateDB()
    {
        string dbPath = System.IO.Path.Combine(Application.persistentDataPath, dbName + ".db");
        string connectionString = $"Data Source={dbPath}";
        
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            
            //Set up an object called command to allow db control
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Scores (P1_Score INTEGER, P2_Score INTEGER, Score_Message TEXT, Time TEXT);";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public static void InsertScore(int P1Score, int P2Score, string time, string scoreMessage)
    {
        Debug.Log("Insert() runs");
        string dbPath = System.IO.Path.Combine(Application.persistentDataPath, dbName + ".db");
        string connectionString = $"Data Source={dbPath}";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {  
                command.CommandText = $"INSERT INTO Scores (P1_Score, P2_Score, Score_Message, Time) VALUES (@P1Score, @P2Score, @scoreMessage, @Time);";
                
                command.Parameters.AddWithValue("@P1Score", P1Score);
                command.Parameters.AddWithValue("@P2Score", P2Score);
                command.Parameters.AddWithValue("@ScoreMessage", scoreMessage);
                command.Parameters.AddWithValue("@Time", time);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public void UpdateScore(int P1Score, int P2Score, Text time)
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
