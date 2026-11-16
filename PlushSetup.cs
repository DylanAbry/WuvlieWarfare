using UnityEngine;
using UnityEngine.AI;

public class PlushSetup : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolPointSampleDistance = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        foreach (Transform point in patrolPoints)
        {
            if (point == null)
                continue;

            ValidatePatrolPoint(point);

            point.SetParent(null, true);
        }
    }

    private void ValidatePatrolPoint(Transform point)
    {
        int allowedAreas = agent.areaMask;

        if (NavMesh.SamplePosition(
            point.position,
            out NavMeshHit hit,
            patrolPointSampleDistance,
            allowedAreas))
        {
            NavMeshPath path = new NavMeshPath();

            bool pathFound = NavMesh.CalculatePath(
                transform.position,
                hit.position,
                allowedAreas,
                path
            );

            if (pathFound &&
                path.status == NavMeshPathStatus.PathComplete)
            {
                point.position = hit.position;
                return;
            }
        }

        if (NavMesh.SamplePosition(
            transform.position,
            out NavMeshHit fallbackHit,
            2f,
            allowedAreas))
        {
            point.position = fallbackHit.position;
        }
    }
}
