using UnityEngine;

public class PressurePlateTrigger : MonoBehaviour
{
    public Animator pressurePlateAnimator;
    public Animator doorAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("dennis suger");
            pressurePlateAnimator.SetTrigger("PressPlate");
            doorAnimator.SetTrigger("OpenDoor");

        }
    }
}
