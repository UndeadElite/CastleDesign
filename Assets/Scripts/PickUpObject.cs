using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public GameObject PickUpText;
    public GameObject ObjectOnPlayer;
    public static bool playerIsHolding = false;

    void Start()
    {
        ObjectOnPlayer.SetActive(false);
        PickUpText.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickUpText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E) && !playerIsHolding)
            {
                playerIsHolding = true;
                this.gameObject.SetActive(false);
                ObjectOnPlayer.SetActive(true);
                PickUpText.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PickUpText.SetActive(false);
    }
}
