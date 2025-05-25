using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class OscarEnemy : MonoBehaviour
{
    public Transform Player;
    private NavMeshAgent agent;

    int currentHealth;
    public int maxHealth;

    public GameObject attackColliderObject; // Assign in Inspector
    public float attackDuration = 0.5f;
    public float attackCooldown = 3f;
    public int damageToPlayer = 1;
    private bool canAttack = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (attackColliderObject != null)
            attackColliderObject.SetActive(false);
    }

    void Update()
    {
        agent.destination = Player.position;

        // Make Oscar face the player on the Y axis only
        Vector3 lookPos = Player.position - transform.position;
        lookPos.y = 0; // Keep only horizontal rotation
        if (lookPos != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookPos);

        // Example: Attack if close enough and can attack
        if (canAttack && Vector3.Distance(transform.position, Player.position) < 2f)
        {
            StartCoroutine(Attack());
        }
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



    void Awake()
    {
        currentHealth = maxHealth;
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
        if (attackColliderObject != null)
            attackColliderObject.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        if (attackColliderObject != null)
            attackColliderObject.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);

        agent.isStopped = false;
        canAttack = true;
    }
}

