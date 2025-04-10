using UnityEngine;

public class FindParentCollider : MonoBehaviour
{
    Collider ParentCollider;
    void Start()
    {
        ParentCollider = GetComponent<Collider>();
    }

    void Update()
    {
        
    }
}
