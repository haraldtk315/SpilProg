using UnityEngine;

public class ScreenWrap : MonoBehaviour
{

    void Update()
    {
        Vector3 wrapPos = BorderManager.GetWrapPos(transform.position);

        transform.position = wrapPos + transform.forward * transform.position.z;
    }
}
