using UnityEngine;
using UnityEngine.AI;
public class NavCrossbow : MonoBehaviour
{
    public Transform Player;
    private NavMeshAgent agent;
    private bool isMoving = true; // Track movement state

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.enabled && agent.isActiveAndEnabled && isMoving)
        {
            agent.destination = Player.position;
        }
    }

    public void SetAgentMoving(bool moving)
    {
        if (agent == null) return;
        isMoving = moving;
        agent.isStopped = !moving;
    }

    int currentHealth;
    public int maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
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
}
