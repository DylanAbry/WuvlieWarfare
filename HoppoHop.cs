using UnityEngine;
using UnityEngine.AI;


public class HoppoHop : MonoBehaviour
{
    [Header("References")]
    public Transform visualModel;

    [Header("Hop Settings")]
    public float hopHeight = 0.75f;
    public float hopsPerSecond = 2f;

    private NavMeshAgent agent;
    private Vector3 startingLocalPosition;
    private float hopTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (visualModel != null)
        {
            startingLocalPosition = visualModel.localPosition;
        }
    }

    void Update()
    {
        if (visualModel == null || agent == null)
            return;

        bool isMoving =
            agent.velocity.sqrMagnitude > 0.01f &&
            agent.hasPath;

        if (isMoving)
        {
            Hop();
        }
        else
        {
            ResetHop();
        }
    }

    void Hop()
    {
        hopTimer += Time.deltaTime * hopsPerSecond;

        // Produces 010 jump arc repeatedly

        float hopAmount =
            Mathf.Abs(Mathf.Sin(hopTimer * Mathf.PI));

        Vector3 position = startingLocalPosition;

        position.y += hopAmount * hopHeight;

        visualModel.localPosition = position;
    }

    void ResetHop()
    {
        hopTimer = 0f;

        visualModel.localPosition =
            Vector3.Lerp(
                visualModel.localPosition,
                startingLocalPosition,
                Time.deltaTime * 10f
            );
    }
}
