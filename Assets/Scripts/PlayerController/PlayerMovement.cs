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

    [Header("Player Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;    

    [Header("Attacking")]
    public float attackDistance = 3f;
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

    [Header("PlayerUI")]
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private GameObject swordObject;

    public GameObject hitEffect;
    public AudioSource swordSwing;
    public AudioSource hitEnemy;
    public AudioSource hitStone;
    public AudioSource hitWood;
    public AudioSource hitGlass;
    private bool hasSword = false;

    private bool attacking = false;
    private bool readyToAttack = true;
 
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public bool isCrouching = false;

    private InputManager inputManager;
    private Transform cameraTransform;
    private Animator animator;
    private AudioSource audioSource;

    private const float CrouchRaycastOffset = 0.1f;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        if (playerUI != null)
            playerUI.SetHealth(currentHealth, maxHealth);

        if (cameraTransform == null)
            Debug.LogError("Main Camera is missing!");
        if (animator == null)
            Debug.LogError("Animator component is missing!");
        swordObject.SetActive(false);
    }
    // Method to get current health
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        Debug.Log("Player took damage: " + damage + ", Current Health: " + currentHealth);
        if (playerUI != null)
            playerUI.SetHealth(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die ()
    {
        isDead = true;
        Debug.Log("Player is dead!");
        // turn off player movement
        enabled = false;
        controller.enabled = false;
        // show Cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void PickupSword()
    {
        hasSword = true;
        EquipSword();
    }
    public void EquipSword()
    {
        if (hasSword && swordObject != null)
            swordObject.SetActive(true);
    }
    public void UnequipSword()
    {
        if (swordObject != null)
            swordObject.SetActive(false);
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Missing AudioSource component on player!");
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraTransform = Camera.main.transform;
        if (cameraTransform == null)
            Debug.LogError("Main Camera is missing!");
    }
    private void Update()
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

        SetAnimations(move);

        if (Input.GetKeyDown(KeyCode.Alpha1) && hasSword)
        {
            EquipSword();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && hasSword)
        {
            UnequipSword();
        }
    }
    private Coroutine crouchCoroutine;
    private void ToggleCrouch()
    {
        if (crouchCoroutine != null)
        {
            StopCoroutine(crouchCoroutine);
        }
        if (isCrouching)
        {
            if (!Physics.Raycast(transform.position, Vector3.up, normalHeight - crouchHeight + CrouchRaycastOffset))
            {
                crouchCoroutine = StartCoroutine(SmoothCrouchTransition(controller.height, normalHeight));
                isCrouching = false;
            }
        }
        else
        {
            crouchCoroutine = StartCoroutine(SmoothCrouchTransition(controller.height, crouchHeight));
            isCrouching = true;
        }
    }
    private System.Collections.IEnumerator SmoothCrouchTransition(float startHeight, float targetHeight)
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            controller.height = Mathf.Lerp(startHeight, targetHeight, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        controller.height = targetHeight;
    }
    public void Attack()
    {
        if (!readyToAttack || attacking) return;
        StartCoroutine(PerformAttack());
    }
    private System.Collections.IEnumerator PerformAttack()
    {
        readyToAttack = false;
        attacking = true;
        animator.SetBool("isAttacking", true);

        audioSource.pitch = Random.Range(0.9f, 1.1f);

        // Only play sword swing sound if sword is equipped
        if (swordObject != null && swordObject.activeSelf)
        {
            swordSwing.Play();
        }

        AttackRaycast();

        yield return new WaitForSeconds(attackSpeed - attackDelay);

        attacking = false;
        animator.SetBool("isAttacking", false);
        readyToAttack = true;
    }
    public void AttackRaycast()
    {
        Vector3 rayOrigin = cameraTransform.position;
        Vector3 rayDirection = cameraTransform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * attackDistance, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, attackDistance, attackLayer))
        {
            HitTarget(hit);

            BreakableScript breakable = hit.transform.GetComponent<BreakableScript>();
            if (breakable != null)
            {
                breakable.Interact();
            }

            // Only deal damage or interact if sword is equipped
            if (swordObject != null && swordObject.activeSelf)
            {
           

                // Deal damage to enemies
                if (hit.transform.CompareTag("Enemy"))
                {
                    NavigationScript target = hit.transform.GetComponent<NavigationScript>();
                    if (target != null)
                    {
                        target.TakeDamage(attackDamage);
                    }
                }
            }
        }
    }
    private void HitTarget(RaycastHit hit)
    {
        audioSource.pitch = 1;

        // Play different sounds based on the object's tag
        switch (hit.transform.tag)
        {
            case "Enemy":
                if (hitEnemy != null) hitEnemy.Play();
                break;
            case "Stone":
                if (hitStone != null) hitStone.Play();
                break;
            case "Wood":
                if (hitWood != null) hitWood.Play();
                break;
            case "Glass":
                if (hitGlass != null) hitGlass.Play();
                break;
        }

        GameObject GO = Instantiate(hitEffect, hit.point, Quaternion.identity);
        Destroy(GO, 10);
    }
    private void SetAnimations(Vector3 move)
    {
        bool walking = move.magnitude >= 0.1f && !attacking;
        animator.SetBool("isWalking", walking);
        animator.SetBool("isAttacking", attacking);
    }
}
