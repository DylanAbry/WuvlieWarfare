using UnityEngine;

public class PatrolZone : MonoBehaviour
{
    private BoxCollider zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<BoxCollider>();

        if (zoneCollider == null)
        {
            Debug.LogError(gameObject.name + " does not have a BoxCollider!");
        }
    }

    public bool IsInsideZone(Vector3 position)
    {
        if (zoneCollider == null)
            return false;

        Bounds bounds = zoneCollider.bounds;

        return position.x >= bounds.min.x &&
               position.x <= bounds.max.x &&
               position.z >= bounds.min.z &&
               position.z <= bounds.max.z;
    }
}
