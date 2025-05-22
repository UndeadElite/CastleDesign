using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Health Images")]
    public Image healthFull;
    public Image healthHalf;
    public Image healthLow;
    public Image healthEmpty;

    public void SetHealth(int current, int max)
    {
        // Hide all images first
        healthFull.enabled = false;
        healthHalf.enabled = false;
        healthLow.enabled = false;
        healthEmpty.enabled = false;

        // Show the correct image based on current health (0-3)
        switch (current)
        {
            case 3:
                healthFull.enabled = true;
                break;
            case 2:
                healthHalf.enabled = true;
                break;
            case 1:
                healthLow.enabled = true;
                break;
            default:
                healthEmpty.enabled = true;
                break;
        }
    }

}