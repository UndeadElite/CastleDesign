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
    public AudioSource swordSwing;
    public AudioSource hitSound;

    private bool attacking = false;
    private bool readyToAttack = true;
    private int attackCount;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public bool isCrouching = false;

    private InputManager inputManager;
    private Transform cameraTransform;
    private Animator animator;
    private AudioSource audioSource;

    private const float CrouchRaycastOffset = 0.1f;

    private enum AnimationState { Idle, Walk, Attack1, Attack2 }
    private AnimationState currentAnimationState;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null)
            Debug.LogError("Main Camera is missing!");
        if (animator == null)
            Debug.LogError("Animator component is missing!");
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
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        Debug.Log("Playing swordSwing sound");
        swordSwing.Play();

        if (attackCount == 0)
        {
            ChangeAnimationState(AnimationState.Attack1);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(AnimationState.Attack2);
            attackCount = 0;
        }

        yield return new WaitForSeconds(attackDelay);
        AttackRaycast();

        yield return new WaitForSeconds(attackSpeed - attackDelay);
        attacking = false;
        readyToAttack = true;
    }

    private void AttackRaycast()
    {
        Vector3 rayOrigin = cameraTransform.position;
        Vector3 rayDirection = cameraTransform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * attackDistance, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, attackDistance, attackLayer))
        {
            HitTarget(hit.point);

            if (hit.transform.TryGetComponent<NavigationScript>(out NavigationScript target))
            {
                target.TakeDamage(attackDamage);
            }
        }
    }

    private void HitTarget(Vector3 pos)
    {
        audioSource.pitch = 1;
        Debug.Log("Playing hitSound sound");
       hitSound.Play();

       

        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
        Destroy(GO, 20);
    }

    private void ChangeAnimationState(AnimationState newState)
    {
        if (currentAnimationState == newState) return;

        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(newState.ToString(), 0.2f);
    }

    private void SetAnimations(Vector3 move)
    {
        if (!attacking)
        {
            if (move.magnitude < 0.1f)
            {
                ChangeAnimationState(AnimationState.Idle);
            }
            else
            {
                ChangeAnimationState(AnimationState.Walk);
            }
        }
    }
}
