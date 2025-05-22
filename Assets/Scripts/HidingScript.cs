using UnityEngine;

public class HidingScript : MonoBehaviour
{
    PlayerMovement playermovement;
    public Collider hidingcollider;

    public bool isHiding = false;

    void Start()
    {
        playermovement = GetComponent<PlayerMovement>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HidingSpot"))
        {
            if (playermovement.isCrouching)
            {
                Debug.Log("Hiding...");
                isHiding = true;
            }
            else
            {
                isHiding = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HidingSpot"))
        {
            Debug.Log("Not hiding");
            isHiding = false;
        }
    }
}
