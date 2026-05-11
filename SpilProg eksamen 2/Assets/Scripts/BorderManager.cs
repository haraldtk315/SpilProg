using UnityEngine;

public class BorderManager : MonoBehaviour
{

    public static BorderManager instance;

    public float leftLimit = -5f;
    public float rightLimit = 5f;
    public float topLimit = 5f;
    public float bottomLimit = -5f;

    public static Vector2 size => new Vector2(-instance.leftLimit + instance.rightLimit, -instance.bottomLimit + instance.topLimit);

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public static Vector2 GetWrapPos(Vector2 pos)
    {
        Vector2 wrapPos;

        wrapPos = new Vector2(
            ((pos.x - (pos.x < 0 ? instance.rightLimit : instance.leftLimit)) % size.x) + (pos.x < 0 ? instance.rightLimit : instance.leftLimit), 
            ((pos.y - (pos.y < 0 ? instance.topLimit : instance.bottomLimit)) % size.y) + (pos.y < 0 ? instance.topLimit : instance.bottomLimit)
            );

        return wrapPos;
        //if (pos.x < leftLimit)
        //    pos.x = rightLimit;
        //else if (pos.x > rightLimit)
        //    pos.x = leftLimit;

        //if (pos.y < bottomLimit)
        //    pos.y = topLimit;
        //else if (pos.y > topLimit)
        //    pos.y = bottomLimit;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(leftLimit, topLimit, 0), new Vector3(rightLimit, topLimit, 0));
        Gizmos.DrawLine(new Vector3(rightLimit, topLimit, 0), new Vector3(rightLimit, bottomLimit, 0));
        Gizmos.DrawLine(new Vector3(rightLimit, bottomLimit, 0), new Vector3(leftLimit, bottomLimit, 0));
        Gizmos.DrawLine(new Vector3(leftLimit, bottomLimit, 0), new Vector3(leftLimit, topLimit, 0));
    }


}
