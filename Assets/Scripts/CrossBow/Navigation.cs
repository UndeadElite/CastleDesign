using UnityEngine;
using UnityEngine.AI;
public class NavigationScript : MonoBehaviour
{
    public Transform Player;
    private NavMeshAgent agent;

    int currentHealth;
    public int maxHealth;

   // BreakableScript breakableScript;

    void Start()
    {
       // breakableScript = GetComponent<BreakableScript>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        agent.destination = Player.position;
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
        // Death function
        // TEMPORARY: Destroy Object
       // breakableScript.Explode();
        Destroy(gameObject);
    }
}
