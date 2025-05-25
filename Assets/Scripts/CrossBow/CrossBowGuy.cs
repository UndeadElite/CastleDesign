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
    [SerializeField] private float retreatDistance = 3f; // Distance to start moving back
    [SerializeField] private LayerMask visionMask; // Set this in the Inspector to include walls and player
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolPointTolerance = 0.5f;

    private HidingScript playerHidingScript;
    private PlayerMovement playerMovement;

    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip dieClip;
    [SerializeField] private AudioClip reloadClip; 
    private AudioSource audioSource;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private Transform player;
    private float attackTimer = 0f;
    private EnemyDrop enemyDrop;
    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;

    private void Awake()
    {
        enemyDrop = GetComponent<EnemyDrop>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // Find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHidingScript = playerObj.GetComponent<HidingScript>();
        }

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
            if (distance < retreatDistance)
            {
                // Move backward from the player
                Vector3 dirFromPlayer = (transform.position - player.position).normalized;
                Vector3 retreatTarget = transform.position + dirFromPlayer * 2f; // Move back 2 units
                agent.isStopped = false;
                agent.destination = retreatTarget;
                AimAtPlayer();
            }
            else if (distance < stopDistance)
            {
                // Stop, aim, and attack
                agent.isStopped = true;
                AimAtPlayer();
                AttackPlayer();
            }
            else
            {
                // Chase the player
                agent.isStopped = false;
                agent.destination = player.position;
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
        // If player is hiding, enemy cannot see them
        if (playerHidingScript != null && playerHidingScript.isHiding)
            return false;

        Vector3 origin = transform.position + Vector3.up * 1.0f; 
        Vector3 direction = (player.position + Vector3.up * 1.0f) - origin;
        float distance = Vector3.Distance(origin, player.position + Vector3.up * 1.0f);

       
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, visionMask))
        {
          
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
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            rb.linearVelocity = firePoint.forward * arrowSpeed;

            if (shootClip != null && audioSource != null)
                audioSource.PlayOneShot(shootClip);

            // Play reload sound after shooting
            if (reloadClip != null && audioSource != null)
                audioSource.PlayOneShot(reloadClip);

            attackTimer = 0f;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (dieClip != null && audioSource != null)
            audioSource.PlayOneShot(dieClip);

        if (enemyDrop != null)
            enemyDrop.Drop();

        Destroy(gameObject, dieClip != null ? dieClip.length : 0f);
    }
}

