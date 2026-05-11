using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.ComponentModel;

public class ScoreBoardManager : NetworkBehaviour
{

    public static ScoreBoardManager instance;

    public static NetworkVariable<int> score = new NetworkVariable<int>(0);

    [SerializeField] TMPro.TMP_Text scoreText;

    [ServerRpc]
    public void AddScoreServerRpc()
    {
        score.Value++;
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

}
