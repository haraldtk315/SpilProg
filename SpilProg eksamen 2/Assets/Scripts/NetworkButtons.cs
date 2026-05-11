using Unity.Netcode;
using UnityEngine;

public class NetworkButtons : MonoBehaviour
{
    
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
    }

}
