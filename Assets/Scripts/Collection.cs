using UnityEngine;

public class Collection : MonoBehaviour
{
 
    public static Collection Instance { get; private set; }
    private int totalCoins = 0;

    private void Awake()
    {
        // Ensure there's only one instance of the "Collection"
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        Debug.Log("Total Coins: " + totalCoins);
    }

    public int GetTotalCoins()
    {
        return totalCoins;
    }
}
