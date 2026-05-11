using UnityEngine;

public class StarBehaviour : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ScoreBoardManager.instance.AddScoreServerRpc();
    }

}
