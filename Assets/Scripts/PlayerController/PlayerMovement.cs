using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float crouchSpeed = 2.0f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    [Header("Crouch Settings")]
    [SerializeField] private float normalHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.2f;
    
    [Header("Attacking")]
    public float attackDistance = 3f;
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool isCrouching = false;

    private InputManager inputManager;
    private Transform cameraTransform;
    Animator animator;
    AudioSource audioSource;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Missing AudioSource component on player!");
        }

    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector2 movementInput = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movementInput.x, 0f, movementInput.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;

        if (inputManager.PlayerAttackedThisFrame())
        {
            Attack();
        }

        if (inputManager.PlayerCrouchedThisFrame())
        {
            ToggleCrouch();
        }

        float currentSpeed = isCrouching ? crouchSpeed : walkSpeed;
        controller.Move(move * Time.deltaTime * currentSpeed);

        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer && !isCrouching)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void ToggleCrouch()
    {
        if (isCrouching)
        {
           
                    
            if (!Physics.Raycast(transform.position, Vector3.up, normalHeight - crouchHeight + 0.1f))
            {
                controller.height = normalHeight;
                isCrouching = false;
            }
        }
        else
        {
            controller.height = crouchHeight;
            isCrouching = true;
        }
    }



    public void Attack()
    {
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        //audioSource.pitch = Random.Range(0.9f, 1.1f);
        //audioSource.PlayOneShot(swordSwing);

        if (attackCount == 0)
        {

            attackCount++;
        }
        else
        {
            attackCount = 0;
        }
    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    void AttackRaycast()
    {
        Vector3 rayOrigin = cameraTransform.position;
        Vector3 rayDirection = cameraTransform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * attackDistance, Color.red, 1f);
        Debug.Log("1");

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, attackDistance, attackLayer))
        {
            Debug.Log("Hit: " + hit.collider.name);
            HitTarget(hit.point);

            if (hit.transform.TryGetComponent<NavigationScript>(out NavigationScript target))
            {
                target.TakeDamage(attackDamage);
            }
        }
        else
        {
            Debug.Log("No target hit.");
        }
    }




    void HitTarget(Vector3 pos)
    {
        //audioSource.pitch = 1;
        //audioSource.PlayOneShot(hitSound);

        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
        
    }
}


