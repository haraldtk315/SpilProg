using Unity.Netcode;
using UnityEngine;

public class MultiplayerMenu : MonoBehaviour
{
    [Header("Menu UI")]
    [SerializeField] private GameObject menuRoot;

    public void HostGame()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("No NetworkManager found in the scene.");
            return;
        }

        NetworkManager.Singleton.StartHost();
        HideMenu();
    }

    public void JoinGame()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("No NetworkManager found in the scene.");
            return;
        }

        NetworkManager.Singleton.StartClient();
        HideMenu();
    }

    public void StopGame()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.Shutdown();

        if (menuRoot != null)
            menuRoot.SetActive(true);
    }

    private void HideMenu()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}