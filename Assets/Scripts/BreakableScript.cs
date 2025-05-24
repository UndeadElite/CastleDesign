using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BreakableScript : MonoBehaviour, IInteractable
{
    public GameObject originalObject;
    public float explosionMinForce = 5;
    public float explosionMaxForce = 100;
    public float explosionForceRadius = 10;

    AudioSource audioSource;
    public AudioClip ExplosionSFX;
    [SerializeField] List<Rigidbody> rbs;
    public void Interact()
    {
        Explode();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Explode()
    {
        GetComponent<BoxCollider>().enabled = false;

        audioSource.PlayOneShot(ExplosionSFX);

        foreach (Rigidbody piece in rbs)
        {
            piece.isKinematic = false;
            piece.AddExplosionForce(Random.Range(explosionMinForce, explosionMaxForce), originalObject.transform.position, explosionForceRadius);
        }
    }
}
