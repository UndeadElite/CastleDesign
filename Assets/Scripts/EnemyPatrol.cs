using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    private NavMeshAgent agent;
    private Transform currentTarget;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentTarget = pointA;
        MoveToNextPoint();
    }

    void Update()
    {
        if (!isWaiting && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAndSwitchTarget());
        }
    }

    IEnumerator WaitAndSwitchTarget()
    {
        isWaiting = true;
        yield return new WaitForSeconds(0.1f); // Wait for 1 second
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        MoveToNextPoint();
        isWaiting = false;
    }

    void MoveToNextPoint()
    {
        if (agent != null && currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
    }
}