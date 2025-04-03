using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] Animator optionMenuAnimator;


    public void OpenOptions()
    {
        optionMenuAnimator.SetBool("optionMenuIsActive", true);

    }

    public void CloseOptions()
    {
        optionMenuAnimator.SetBool("optionMenuIsActive", false);

    }

}