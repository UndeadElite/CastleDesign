using UnityEngine;
using UnityEngine.AI;
public class NavigationScript : MonoBehaviour
{
    public Transform Player;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.enabled && agent.isActiveAndEnabled)
        {
            agent.destination = Player.position;
        }
    }

    public void SetAgentMoving(bool moving)
    {
        if (agent == null) return;
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
        // Death function
        // TEMPORARY: Destroy Object
        Destroy(gameObject);
    }
}
