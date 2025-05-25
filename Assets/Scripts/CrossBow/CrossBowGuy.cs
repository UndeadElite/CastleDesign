using UnityEngine;
using UnityEngine.AI;

public class CrossBowGuy : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 4f;
    [SerializeField] private Transform firePoint; // point where the arrow will be fired
    [SerializeField] private float arrowSpeed = 20f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float stopDistance = 6f; // Distance to stop and aim
    [SerializeField] private LayerMask visionMask; // Set this in the Inspector to include walls and player
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolPointTolerance = 0.5f;

    private Transform player;
    private float attackTimer = 0f;
    private EnemyDrop enemyDrop;
    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;

    private void Awake()
    {
        enemyDrop = GetComponent<EnemyDrop>();
    }

    private void Start()
    {
        // Find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && HasLineOfSight())
        {
            if (distance > stopDistance)
            {
                // Chase the player
                agent.isStopped = false;
                agent.destination = player.position;
            }
            else
            {
                // Stop, aim, and attack
                agent.isStopped = true;
                AimAtPlayer();
                AttackPlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        agent.isStopped = false;
        if (agent.remainingDistance < patrolPointTolerance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.destination = patrolPoints[currentPatrolIndex].position;
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f; // Adjust height as needed
        Vector3 direction = (player.position + Vector3.up * 1.0f) - origin;
        float distance = Vector3.Distance(origin, player.position + Vector3.up * 1.0f);

        // Raycast only hits objects on visionMask (set to include walls and player)
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, visionMask))
        {
            // Only see the player if the first thing hit is the player
            return hit.transform == player;
        }
        return false;
    }

    private void AimAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void AttackPlayer()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            // Fire an arrow
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            rb.linearVelocity = firePoint.forward * arrowSpeed; // Use linearVelocity as required

            attackTimer = 0f; // Reset the attack timer
        }

    }

    private void Die()
    {
        if (enemyDrop != null)
            enemyDrop.Drop();

        Destroy(gameObject);
    }

    public void OnPlayerDetected(Transform player)
    {
        // Start aiming and attacking
    }

    public void OnPlayerLost()
    {
        // Stop attacking, resume patrol, etc.
    }
}
