using UnityEngine;

public class Collection : MonoBehaviour
{
    [SerializeField] private int value = 1;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            Destroy(gameObject);
        }
    }

  
}


