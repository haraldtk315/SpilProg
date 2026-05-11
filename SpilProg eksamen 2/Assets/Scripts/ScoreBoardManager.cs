using UnityEngine;
using Unity.Netcode;

public class ScoreBoardManager : NetworkBehaviour
{

    public static ScoreBoardManager instance;

    public static NetworkVariable<int> score = new NetworkVariable<int>(0);

    [SerializeField] private JSONSystemIO jsonSystem;
    [SerializeField] TMPro.TMP_Text scoreText;

    [ServerRpc]
    public void AddScoreServerRpc()
    {
        score.Value++;
        jsonSystem.SaveScore(score.Value);

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
    
    public void Update()
    {
        scoreText.text = score.Value.ToString();

        /*
        InputAction jump = InputSystem.actions.FindAction("Crouch");
        if (jump.ReadValue<float>() > 0 && jump.triggered)
        {
            AddScoreServerRpc();
        }
        */
    }
    public void SaveCurrentScore(int currentScore)
    {
        jsonSystem.SaveScore(currentScore);
    }

    public void LoadSavedScore()
    {
        if (IsServer)
        {
            score.Value = jsonSystem.LoadScore();
        }
    }



}
