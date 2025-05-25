using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private GameObject[] dropPrefabs;
    [SerializeField] private float dropChance = 1.0f;  // 1.0 = 100% chance, 0.5 = 50% chance

    // will only work if enemy is destroyed :(
    public void Drop()
    {
        if (dropPrefabs.Length == 0) return;

        if (Random.value <= dropChance)
        {
            // Pick a random item to drop
            int index = Random.Range(0, dropPrefabs.Length);
            Instantiate(dropPrefabs[index], transform.position, Quaternion.identity);
        }
    }
}
