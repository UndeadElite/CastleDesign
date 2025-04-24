using UnityEngine;

public class CrossBowGuy : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 4f;
    [SerializeField] Transform firePoint;// point where the arrow will be fired
    [SerializeField] float arrowSpeed = 20f;
    [SerializeField] GameObject arrowPrefab;

    private Transform player;
    private int currentPatrolIndex = 0;
    private float attackTimer = 0f;

    private void AttackPlayer()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            // Fire an arrow
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            rb.linearVelocity = firePoint.forward * arrowSpeed;

            attackTimer = 0f; // Reset the attack timer
        }
    }


}
