using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    public float leftLimit = -5f;
    public float rightLimit = 5f;
    public float topLimit = 5f;
    public float bottomLimit = -5f;

    void Update()
    {
        Vector3 pos = transform.position;

        if (pos.x < leftLimit)
            pos.x = rightLimit;
        else if (pos.x > rightLimit)
            pos.x = leftLimit;

        if (pos.y < bottomLimit)
            pos.y = topLimit;
        else if (pos.y > topLimit)
            pos.y = bottomLimit;

        transform.position = pos;
    }
}
