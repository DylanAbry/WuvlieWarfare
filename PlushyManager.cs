using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;
using Random = UnityEngine.Random;

public class PlushyManager : MonoBehaviour
{
    [Header("Plush Setup")]
    public GameObject rattlemeowserPrefab;
    public PatrolZone[] rattlemeowserZones;

    public GameObject pinchypupPrefab;
    public PatrolZone[] pinchypupZones;

    public GameObject hoppoPrefab;
    public PatrolZone[] hoppoZones;

    public GameObject shellsharkPrefab;
    public Transform[] shellsharkPatrolPoints;

    public GameObject player;

    [Header("NavMesh")]
    public float navMeshSampleDistance = 3f;

    public float spawnRate = 10f;
    public int plushyCapacity = 20;

    private HashSet<GameObject> livingPlushies = new HashSet<GameObject>();

    public int CurrentPlushyCount => livingPlushies.Count;

    public PlayerManager playerScript;

    void Start()
    {
        spawnRate = 10;
        plushyCapacity = 20;
        InvokeRepeating("AddPlushy", 5, spawnRate);
        
    }

    public void SpawnRattlemeowser()
    {
        int zoneIndex = Random.Range(0, rattlemeowserZones.Length);
        
        if (rattlemeowserPrefab == null)
            return;

        BoxCollider zoneCollider = rattlemeowserZones[zoneIndex].GetComponent<BoxCollider>();

        Vector3 localPoint = new Vector3(
            Random.Range(-0.5f, 0.5f),
            0f,
            Random.Range(-0.5f, 0.5f)
        );

        localPoint.x *= zoneCollider.size.x;
        localPoint.z *= zoneCollider.size.z;

        localPoint += zoneCollider.center;

        Vector3 desiredWorldPosition =
            zoneCollider.transform.TransformPoint(localPoint);

        if (NavMesh.SamplePosition(desiredWorldPosition, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            GameObject newRattlemeowser = Instantiate(rattlemeowserPrefab, hit.position, Quaternion.identity);
            BehaviorGraphAgent agent = newRattlemeowser.GetComponent<BehaviorGraphAgent>();
            if (agent != null)
            {
                bool assigned = agent.SetVariableValue("Player", player);
            }
            RegisterPlushy(newRattlemeowser);
        }
        else
        {
            Debug.Log(
                "Could not find a valid NavMesh spawn point in " +
                rattlemeowserZones[zoneIndex].name
            );
        }
    }

    public void SpawnPinchypup()
    {
        int zoneIndex = Random.Range(0, pinchypupZones.Length);

        if (pinchypupPrefab == null)
            return;

        BoxCollider zoneCollider = pinchypupZones[zoneIndex].GetComponent<BoxCollider>();

        Vector3 localPoint = new Vector3(
            Random.Range(-0.5f, 0.5f),
            0f,
            Random.Range(-0.5f, 0.5f)
        );

        localPoint.x *= zoneCollider.size.x;
        localPoint.z *= zoneCollider.size.z;

        localPoint += zoneCollider.center;

        Vector3 desiredWorldPosition =
            zoneCollider.transform.TransformPoint(localPoint);

        if (NavMesh.SamplePosition(desiredWorldPosition, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            GameObject newPinchypup = Instantiate(pinchypupPrefab, hit.position, Quaternion.identity);
            BehaviorGraphAgent agent = newPinchypup.GetComponent<BehaviorGraphAgent>();
            if (agent != null)
            {
                bool assigned = agent.SetVariableValue("Player", player);
            }
            RegisterPlushy(newPinchypup);
        }
        else
        {
            Debug.Log(
                "Could not find a valid NavMesh spawn point in " +
                pinchypupZones[zoneIndex].name
            );
        }
    }

    public void SpawnHoppo()
    {
        int zoneIndex = Random.Range(0, hoppoZones.Length);

        if (hoppoPrefab == null)
            return;

        BoxCollider zoneCollider = hoppoZones[zoneIndex].GetComponent<BoxCollider>();

        Vector3 localPoint = new Vector3(
            Random.Range(-0.5f, 0.5f),
            0f,
            Random.Range(-0.5f, 0.5f)
        );

        localPoint.x *= zoneCollider.size.x;
        localPoint.z *= zoneCollider.size.z;

        localPoint += zoneCollider.center;

        Vector3 desiredWorldPosition =
            zoneCollider.transform.TransformPoint(localPoint);

        if (NavMesh.SamplePosition(desiredWorldPosition, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            GameObject newHoppo = Instantiate(hoppoPrefab, hit.position, Quaternion.identity);
            BehaviorGraphAgent agent = newHoppo.GetComponent<BehaviorGraphAgent>();
            if (agent != null)
            {
                bool assigned = agent.SetVariableValue("Player", player);
            }
            RegisterPlushy(newHoppo);
        }
        else
        {
            Debug.Log(
                "Could not find a valid NavMesh spawn point in " +
                hoppoZones[zoneIndex].name
            );
        }
    }

    public void SpawnShellshark()
    {
        if (shellsharkPrefab == null ||
            shellsharkPatrolPoints == null ||
            shellsharkPatrolPoints.Length == 0)
        {
            return;
        }

        int spawnIndex =
            Random.Range(0, shellsharkPatrolPoints.Length);

        Transform spawnPoint =
            shellsharkPatrolPoints[spawnIndex];

        GameObject newShellshark = Instantiate(
            shellsharkPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        BehaviorGraphAgent agent =
            newShellshark.GetComponent<BehaviorGraphAgent>();

        if (agent != null)
        {
            
            agent.SetVariableValue("Player", player);

            
            for (int i = 0; i < shellsharkPatrolPoints.Length; i++)
            {
                int routeIndex =
                    (spawnIndex + 1 + i)
                    % shellsharkPatrolPoints.Length;

                agent.SetVariableValue(
                    "PatrolPoint" + (i + 1),
                    shellsharkPatrolPoints[routeIndex].gameObject
                );
            }
        }

        RegisterPlushy(newShellshark);
    }

    public void AddPlushy()
    {
        if (playerScript.gameOver) return;

        if (livingPlushies.Count >= plushyCapacity)
            return;

        int plushyIndex = Random.Range(0, 4);

        switch (plushyIndex)
        {
            case 0:
                SpawnRattlemeowser();
                break;

            case 1:
                SpawnPinchypup();
                break;

            case 2:
                SpawnHoppo();
                break;

            case 3:
                SpawnShellshark();
                break;
        }
    }

    private void RegisterPlushy(GameObject plushy)
    {
        if (plushy == null) return;

        livingPlushies.Add(plushy);

        PlushyPopulationMember member =
            plushy.AddComponent<PlushyPopulationMember>();

        member.Initialize(this);
    }

    public void RemovePlushy(GameObject plushy)
    {
        livingPlushies.Remove(plushy);
    }
}
