using Unity.Netcode;
using UnityEngine;

public class CameraFollowLocalPlayer : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float followSpeed = 10f;

    private Transform target;

    private void LateUpdate()
    {
        if (target == null)
            FindLocalPlayer();

        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }

    private void FindLocalPlayer()
    {
        NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();

        foreach (NetworkObject networkObject in networkObjects)
        {
            if (!networkObject.IsPlayerObject)
                continue;

            if (!networkObject.IsOwner)
                continue;

            target = networkObject.transform;
            return;
        }
    }
}