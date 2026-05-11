using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class ScoreBoardManager : NetworkBehaviour
{

    public static NetworkVariable<int> score = new NetworkVariable<int>(0);

    [SerializeField] TMPro.TMP_Text scoreText;

    [ServerRpc]
    public void AddScoreServerRpc()
    {
        score.Value++;
    }

    public void Update()
    {
        scoreText.text = score.Value.ToString();

        InputAction jump = InputSystem.actions.FindAction("Jump");
        if (jump.ReadValue<float>() > 0 && jump.triggered)
        {
            AddScoreServerRpc();
        }
    }

}
