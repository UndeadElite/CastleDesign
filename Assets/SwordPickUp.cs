using UnityEngine;

public class SwordPickUp : MonoBehaviour
{
    public PlayerMovement player; // Assign in Inspector or find at runtime

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player == null)
                player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.PickupSword();
                Destroy(gameObject); // Remove sword from scene
            }
        }
    }
}
