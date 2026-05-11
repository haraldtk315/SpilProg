using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using UnityEditor.MemoryProfiler;

public class DataBaseManager : MonoBehaviour
{
    public int P1Score;
    public int P2Score;
    public Text time;
    public Text scoreMessage;
    
    //Name of the DB
    private string dbName = "SpilProg2ExamDatabase";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateDB();
    }
    
    public void CreateDB()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            Connection.Open();
            
            //Set up an object called command to allow db control
            using (var command = Connection.CreateCommand())
            {
                command CommandText = "CREATE TABLE IF NOT EXISTS Scores (P1_Score INTEGER, P2_Score INTEGER, Score_Message TEXT, Time TEXT);";
                Command.ExecuteNonQuery();
            }
            Connection.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
