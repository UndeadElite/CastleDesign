using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f; // Arrow will be destroyed after this time
    [SerializeField] private int damage = 10;     // Damage dealt to the player or target

    private void Start()
    {
        // Destroy the arrow after a set time to avoid clutter
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
    
        if (collision.gameObject.CompareTag("Player"))
        {
           
             collision.gameObject.GetComponent<PlayerMovement>()?.TakeDamage(damage);
        }

        
        Destroy(gameObject);
    }
}
