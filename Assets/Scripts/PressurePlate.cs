using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    bool playerIsOn = false;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsOn = true;
            Debug.Log("Player is on");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerIsOn = false;
        Debug.Log("Player is off");
    }
}
