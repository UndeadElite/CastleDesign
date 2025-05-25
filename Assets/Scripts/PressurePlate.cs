using UnityEngine;

public class PressurePlateTrigger : MonoBehaviour
{
    public Animator pressurePlateAnimator;
    public Animator doorAnimator;
    public AudioSource doorCreaking;
    public ParticleSystem doorSmoke; 
    public bool isDoorOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isDoorOpen)
        {
            Debug.Log("Door is already open, ignoring trigger.");
            return;
        }

        isDoorOpen = true; // Set the door state to open

        if (other.CompareTag("Player"))
        {
            Debug.Log("dennis suger");
            pressurePlateAnimator.SetTrigger("PressurePlate1");
            doorAnimator.SetTrigger("OpenDoor");

            if (doorSmoke != null)
            {
                doorSmoke.Play();
            }
            else
            {
                Debug.LogWarning("DoorSmoke ParticleSystem not assigned on PressurePlateTrigger script for " + gameObject.name);
            }
        }

        if (doorCreaking != null)
        {
            doorCreaking.Play();
        }
        else
        {
            Debug.LogWarning("Door Creaking AudioSource not assigned on PressurePlateTrigger script for " + gameObject.name);
        }

    }
    
}
