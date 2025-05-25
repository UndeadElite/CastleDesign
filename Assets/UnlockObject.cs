using UnityEngine;
using static Interactor;


public class UnlockObject : MonoBehaviour, IInteractable
{
    public Animator doorAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Implementation of the IInteractable interface
    public void Interact()
    {
        Debug.Log("UnlockObject has been interacted with.");
        // Check if the player is holding the key
        if (PickUpObject.playerIsHolding)
        {
            // Unlock the door (this object)
            gameObject.SetActive(false);

            // Set the key to not held anymore
            PickUpObject.playerIsHolding = false;

            // Optionally, you can add logic here to open the door or play an animation
            // e.g., DoorObject.SetActive(true);
        }
        else
        {
            Debug.Log("You need a key to unlock this door.");
        }
    }
}
