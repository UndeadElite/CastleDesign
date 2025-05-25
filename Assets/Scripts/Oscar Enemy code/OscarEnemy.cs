using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class OscarEnemy : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Player Detection")]
    public Transform Player;
    public float primaryDetectionRadius = 10f;
    public float primaryFieldOfViewAngle = 90f;
    public float secondaryDetectionRadius = 20f;
    public float secondaryFieldOfViewAngle = 45f;
    public LayerMask viewMask;
    private HidingScript playerHidingScript;



    private NavMeshAgent agent;
    private Transform currentTarget;
    private bool isWaiting = false;
    private bool isChasing = false;
    private bool sawInSecondaryView = false;
    private Coroutine chaseCoroutine;

    int currentHealth;
    public int maxHealth;

    public GameObject attackColliderObject; // Assign in Inspector
    public float attackDuration = 0.5f;
    public float attackCooldown = 3f;
    public int damageToPlayer = 1;
    private bool canAttack = true;


    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (attackColliderObject != null)
            attackColliderObject.SetActive(false);

        if (Player != null)
            playerHidingScript = Player.GetComponent<HidingScript>();

        currentTarget = pointA;
        MoveToNextPoint();
    }


    void Update()
    {
        // Prevent any movement or actions while in attack cooldown
        if (!canAttack)
            return;

        bool currentlySeeingPlayer = CanSeePlayer(out bool currentSeenInSecondary);

        if (currentlySeeingPlayer)
        {
            isChasing = true;
            sawInSecondaryView = currentSeenInSecondary;
            if (chaseCoroutine != null) StopCoroutine(chaseCoroutine);
            agent.isStopped = false;
            agent.SetDestination(Player.position);

            // Face the player
            Vector3 lookPos = Player.position - transform.position;
            lookPos.y = 0;
            if (lookPos != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookPos);

            // Attack if close enough and can attack
            if (canAttack && Vector3.Distance(transform.position, Player.position) < 2f)
            {
                StartCoroutine(Attack());
            }
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
        if (Player == null)
        {
            Debug.LogWarning("Player reference is not assigned!");
            return false;
        }

        if (playerHidingScript != null && playerHidingScript.isHiding)
            return false;

        Vector3 directionToPlayer = Player.position - transform.position;
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
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = directionToPlayer.normalized;
        float maxDistance = Mathf.Max(primaryDetectionRadius, secondaryDetectionRadius);

        Debug.DrawRay(origin, direction * maxDistance, Color.red, 0.1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, viewMask))
        {
            Debug.Log("Raycast hit: " + hit.transform.name);
            if (hit.transform == Player) return true;
        }
        return false;
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
        if (attackColliderObject != null && other.gameObject == Player.gameObject && attackColliderObject.activeSelf)
        {
            var playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(damageToPlayer);
            }
        }
    }


    public void TakeDamage(int amount)
    {
        Debug.Log("Enemy took damage: ");
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }

    IEnumerator Attack()
    {
        canAttack = false;
        agent.isStopped = true;

        // Simple forward placement of collider
        if (attackColliderObject != null)
        {
            attackColliderObject.transform.localPosition = new Vector3(0, 1, 1); // Adjust Y/Z as needed
            attackColliderObject.transform.localRotation = Quaternion.identity;
            attackColliderObject.SetActive(true);
        }

        yield return new WaitForSeconds(attackDuration);

        if (attackColliderObject != null)
            attackColliderObject.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);

        agent.isStopped = false;
        canAttack = true;
    }




}
