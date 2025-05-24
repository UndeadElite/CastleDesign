using UnityEngine;

interface IInteractable
{
    public void Interact();
}
public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public GameObject PickUpText;
    PickUpObject pickUpObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }


        Ray r2 = new Ray(InteractorSource.position, InteractorSource.forward);
        RaycastHit hitInfo2;

        if (Physics.Raycast(r2, out hitInfo2, InteractRange))
        {
            if (hitInfo2.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (hitInfo2.collider.CompareTag("Key"))
                {
                    PickUpText.SetActive(true);
                }
                else
                {
                    PickUpText.SetActive(false);
                }
            }
            else
            {
                PickUpText.SetActive(false);
            }
        }
        else
        {
            PickUpText.SetActive(false);
        }


    }


}
