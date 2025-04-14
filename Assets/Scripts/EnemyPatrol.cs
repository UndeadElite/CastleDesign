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
    public float primaryDetectionRadius = 10f;
    public float primaryFieldOfViewAngle = 90f;
    public float secondaryDetectionRadius = 20f;
    public float secondaryFieldOfViewAngle = 45f;
    public LayerMask viewMask;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private bool isWaiting = false;
    private bool isChasing = false;
    private bool sawInSecondaryView = false;
    private Coroutine chaseCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentTarget = pointA;
        MoveToNextPoint();
    }

    void Update()
    {
        bool currentlySeeingPlayer = CanSeePlayer(out bool currentSeenInSecondary);

        if (currentlySeeingPlayer)
        {
            isChasing = true;
            sawInSecondaryView = currentSeenInSecondary;
            if (chaseCoroutine != null) StopCoroutine(chaseCoroutine);
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else if (isChasing)
        {
            isChasing = false;
            if (chaseCoroutine != null) StopCoroutine(chaseCoroutine);
            chaseCoroutine = StartCoroutine(SearchForPlayerRoutine(sawInSecondaryView));
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
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);
        }
    }

    bool CanSeePlayer(out bool seenInSecondary)
    {
        seenInSecondary = false;
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (distanceToPlayer < primaryDetectionRadius && angle < primaryFieldOfViewAngle * 0.5f)
        {
            if (RayHitsPlayer(directionToPlayer)) return true;
        }
        if (distanceToPlayer < secondaryDetectionRadius && angle < secondaryFieldOfViewAngle * 0.5f)
        {
            if (RayHitsPlayer(directionToPlayer))
            {
                seenInSecondary = true;
                return true;
            }
        }
        return false;
    }

    bool RayHitsPlayer(Vector3 directionToPlayer)
    {
        Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Max(primaryDetectionRadius, secondaryDetectionRadius), viewMask))
        {
            if (hit.transform == player) return true;
        }
        return false;
    }

    IEnumerator StopOnTouch()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;
    }

    IEnumerator SearchForPlayerRoutine(bool sawInSecondary)
    {
        agent.isStopped = true;

        if (sawInSecondary)
        {
            agent.isStopped = false;
            Vector3 forwardDestination = transform.position + transform.forward * 5f;
            agent.SetDestination(forwardDestination);
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f);
            agent.isStopped = true;
        }

        yield return RotateSearch(90);
        yield return RotateSearch(-180);

        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
        agent.isStopped = false;
        MoveToNextPoint();
    }

    IEnumerator RotateSearch(float angle)
    {
        float duration = 0.5f;
        Quaternion fromRotation = transform.rotation;
        Quaternion toRotation = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);
        float time = 0;

        while (time < duration)
        {
            transform.rotation = Quaternion.Slerp(fromRotation, toRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = toRotation;
        yield return new WaitForSeconds(1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player && isChasing)
        {
            StartCoroutine(StopOnTouch());
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, primaryDetectionRadius);
        Vector3 primaryLeft = Quaternion.Euler(0, -primaryFieldOfViewAngle * 0.5f, 0) * transform.forward;
        Vector3 primaryRight = Quaternion.Euler(0, primaryFieldOfViewAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, primaryLeft * primaryDetectionRadius);
        Gizmos.DrawRay(transform.position, primaryRight * primaryDetectionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, secondaryDetectionRadius);
        Vector3 secondaryLeft = Quaternion.Euler(0, -secondaryFieldOfViewAngle * 0.5f, 0) * transform.forward;
        Vector3 secondaryRight = Quaternion.Euler(0, secondaryFieldOfViewAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, secondaryLeft * secondaryDetectionRadius);
        Gizmos.DrawRay(transform.position, secondaryRight * secondaryDetectionRadius);
    }
}
