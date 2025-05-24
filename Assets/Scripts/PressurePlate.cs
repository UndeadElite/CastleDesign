using UnityEngine;

public class PressurePlateTrigger : MonoBehaviour
{
    public Animator pressurePlateAnimator;
    public Animator doorAnimator;
    public AudioSource doorCreaking;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("dennis suger");
            pressurePlateAnimator.SetTrigger("PressurePlate1");
            doorAnimator.SetTrigger("OpenDoor");

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
