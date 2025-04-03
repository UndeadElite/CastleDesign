using UnityEngine;

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

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool isCrouching = false;

    private InputManager inputManager;
    private Transform cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
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
}

