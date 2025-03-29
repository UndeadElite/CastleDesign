using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform cameraTransform;
    private CapsuleCollider playerCollider;

    [Header("Movement Settings")]
    public float speed = 6f;
    public float crouchSpeed = 3f;
    public float jumpForce = 5f;
    public float gravityMultiplier = 2f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;

    [Header("Head Bobbing Settings")]
    public float bobFrequency = 5f;
    public float bobAmplitude = 0.05f;
    private float headBobTimer = 0f;
    private Vector3 originalCameraPosition;

    [Header("Crouching Settings")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchTransitionSpeed = 6f;
    private bool isCrouching = false;

    private bool isGrounded;
    private Vector3 movementInput;
    private float cameraPitch = 0f;

    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        playerCollider = GetComponent<CapsuleCollider>();
        if (!playerCollider) Debug.LogError("CapsuleCollider missing from Player!");

        Cursor.lockState = CursorLockMode.Locked;

        originalCameraPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovementInput();
        HandleJump();
        HandleCrouch();
        HandleHeadBobbing();
    }

    void FixedUpdate()
    {
        MovePlayer();
        ApplyGravity();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
    void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movementInput = new Vector3(horizontal, 0f, vertical).normalized;
    }
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
        }

        float targetHeight = isCrouching ? crouchHeight : standHeight;

        
        playerCollider.height = Mathf.Lerp(playerCollider.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

       
        Vector3 targetCameraPos = originalCameraPosition;
        targetCameraPos.y = isCrouching ? crouchHeight - 0.2f : originalCameraPosition.y;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCameraPos, Time.deltaTime * crouchTransitionSpeed);
    }
    void MovePlayer()
    {
        if (movementInput.magnitude >= 0.1f)
        {
            Vector3 moveDirection = transform.right * movementInput.x + transform.forward * movementInput.z;
            float moveSpeed = isCrouching ? crouchSpeed : speed;
            Vector3 targetVelocity = moveDirection * moveSpeed;

            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }
    void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1), ForceMode.Acceleration);
        }
    }
    void HandleHeadBobbing()
    {
        if (movementInput.magnitude >= 0.1f && isGrounded)
        {
            headBobTimer += Time.deltaTime * bobFrequency;
            float bobOffset = Mathf.Sin(headBobTimer) * bobAmplitude;
            cameraTransform.localPosition += new Vector3(0f, bobOffset, 0f);
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalCameraPosition, Time.deltaTime * 5f);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}



