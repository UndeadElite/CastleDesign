using UnityEngine;
using static Interactor;

public class PickUpObject : MonoBehaviour, IInteractable
{
    public GameObject ObjectOnPlayer;
    public static bool playerIsHolding = false;

    void Start()
    {
        ObjectOnPlayer.SetActive(false);
    }
    //fix only show the text when looking at it
    public void Interact()
    {
        if (!playerIsHolding)
        {
            playerIsHolding = true;
            gameObject.SetActive(false);
            ObjectOnPlayer.SetActive(true);
        }
    }
}
