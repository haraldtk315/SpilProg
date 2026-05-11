using UnityEngine;
using System.Collections;

public class StarBehaviour : MonoBehaviour
{
    public StarManager starManager;

    private void Awake()
    {
        starManager = FindFirstObjectByType<StarManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ScoreBoardManager.instance.AddScoreServerRpc();
        starManager.StartCoroutine(starManager.DestroyStar());
    }
    
}
