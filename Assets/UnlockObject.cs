using UnityEngine;
using static Interactor;


public class UnlockObject : MonoBehaviour, IInteractable
{
    public Animator doorAnimator;
    public AudioSource doorCreaking;
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
            // Play the unlock/open animation if the Animator is assigned
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("OpenDoor");
            }
            else
            {
                Debug.LogWarning("No Animator assigned to UnlockObject.");
            }

            if (doorCreaking != null)
            {
                doorCreaking.Play();
            }
            else
            {
                Debug.LogWarning("Door Creaking AudioSource not assigned on PressurePlateTrigger script for " + gameObject.name);
            }

            // Set the key to not held anymore
            PickUpObject.playerIsHolding = false;
        }
        else
        {
            Debug.Log("You need a key to unlock this door.");
        }
    }
}
