using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Type Flags")]
    public bool isDoor;
    public bool isPickup;

    [Header("Interaction Events")]
    public UnityEvent onDoorInteract;
    public UnityEvent<PickUpObject> onPickupInteract;

    //call ins
    PickUpObject pickUpScript;

    private void Start()
    {
        pickUpScript = GetComponent<PickUpObject>();
    }
    public void Interact()
    {
        if (isDoor)
        {
            Debug.Log("Interacting with Door...");
            onDoorInteract?.Invoke();
        }

        if (isPickup)
        {
            Debug.Log("Interacting with Pickup...");
            onPickupInteract?.Invoke(pickUpScript);
        }
    }
}
