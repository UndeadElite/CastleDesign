using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public GameObject PickUpText;
    public GameObject ObjectOnPlayer;
    public static bool playerIsHolding = false;

    private FindParentCollider parentColliderFinder;

    void Start()
    {
        // Find the ParentColliderFinder script on the parent object
        parentColliderFinder = transform.parent.GetComponent<FindParentCollider>();

        // Disable pickup-related objects initially
        ObjectOnPlayer.SetActive(false);
        PickUpText.SetActive(false);

        if (parentColliderFinder == null)
        {
            Debug.LogError("ParentColliderFinder script not found on parent object.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the player is interacting with the parent collider
        if (other.CompareTag("Player") && parentColliderFinder != null)
        {
            PickUpText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E) && !playerIsHolding)
            {
                PickUp();  // Call the pickup method when E is pressed
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickUpText.SetActive(false);
        }
    }

    // Public method to handle the actual pickup logic
    public void PickUp()
    {
        if (!playerIsHolding)
        {
            playerIsHolding = true;
            this.gameObject.SetActive(false);
            ObjectOnPlayer.SetActive(true);
            PickUpText.SetActive(false);
        }
    }
}
