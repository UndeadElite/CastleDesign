using UnityEngine;

public class CrossBowGuy : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 4f;
    [SerializeField] private Transform firePoint; // point where the arrow will be fired
    [SerializeField] private float arrowSpeed = 20f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float moveSpeed = 3f;

    private Transform player;
    private float attackTimer = 0f;
    private NavCrossbow navigationScript;

    private void Start()
    {
        // Find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        navigationScript = GetComponent<NavCrossbow>();
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            navigationScript.SetAgentMoving(false); // Stop moving
            AimAtPlayer();
            AttackPlayer();
        }
        else
        {
            navigationScript.SetAgentMoving(true); // Move toward player
        }
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
            rb.linearVelocity = firePoint.forward * arrowSpeed; // Updated to use linearVelocity

            attackTimer = 0f; // Reset the attack timer
        }
    }

    private void MoveTowardsPlayer()
    {
        
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Prevent moving up/down
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Face the player while moving
        AimAtPlayer();
    }
}
