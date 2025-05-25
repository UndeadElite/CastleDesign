using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] Animator optionMenuAnimator;

    bool IsOn = false;

    public void ToggleOptions()
    {
        IsOn = !IsOn;
        optionMenuAnimator.SetBool("optionMenuIsActive", IsOn);
    }

    public void OpenOptions()
    {
        IsOn = true;
        optionMenuAnimator.SetBool("optionMenuIsActive", true);
    }

    public void CloseOptions()
    {
        IsOn = false;
        optionMenuAnimator.SetBool("optionMenuIsActive", false);
    }
}
