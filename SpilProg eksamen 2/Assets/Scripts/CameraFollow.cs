using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] Transform player;
    [Range(0, 1)] [SerializeField] float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.position.x, speed), transform.position.y, transform.position.z);
    }
}
