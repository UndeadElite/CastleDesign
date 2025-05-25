using UnityEngine;

public class Enemy : MonoBehaviour
{
    int EnemyHP = 2;
    PlayerMovement playerMovement;
   private EnemyDrop enemyDrop;

    private void Awake()
    {
        enemyDrop = GetComponent<EnemyDrop>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void TakeDamage()
    {
        
    }

    private void Die()
    {
        if (enemyDrop != null)
            enemyDrop.Drop();

        Destroy(gameObject);
    }
}
