using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Player Detection")]
    public Transform player;
    public float detectionRadius = 10f;
    public float fieldOfViewAngle = 90f;
    public LayerMask viewMask = ~0;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private bool isWaiting = false;
    private bool isChasing = false;
    private Coroutine chaseCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentTarget = pointA;
        MoveToNextPoint();
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            if (!isChasing)
            {
                isChasing = true;
                if (chaseCoroutine != null) StopCoroutine(chaseCoroutine);
                agent.SetDestination(player.position);
            }
            else
            {
                agent.SetDestination(player.position);
            }
        }
        else if (isChasing)
        {
            // Lost sight of player
            isChasing = false;
            chaseCoroutine = StartCoroutine(ResumePatrolAfterDelay());
        }
        else if (!isWaiting && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAndSwitchTarget());
        }
    }

    IEnumerator WaitAndSwitchTarget()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1f);
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        MoveToNextPoint();
        isWaiting = false;
    }

    IEnumerator ResumePatrolAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        MoveToNextPoint();
    }

    void MoveToNextPoint()
    {
        if (agent != null && currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (directionToPlayer.magnitude < detectionRadius && angle < fieldOfViewAngle * 0.5f)
        {
            Debug.Log("Player is within vision cone");
            Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer.normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, detectionRadius, viewMask))
            {
                Debug.Log("Raycast hit: " + hit.transform.name);
                if (hit.transform == player)
                {
                    Debug.Log("Enemy sees the player!");
                    return true;
                }
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player && isChasing)
        {
            StartCoroutine(StopOnTouch());
        }
    }

    IEnumerator StopOnTouch()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 leftLimit = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftLimit * detectionRadius);
        Gizmos.DrawRay(transform.position, rightLimit * detectionRadius);
    }
}
