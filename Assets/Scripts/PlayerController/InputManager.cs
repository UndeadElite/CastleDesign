using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerController playercontroller;

    private static InputManager _instance;

    public static InputManager Instance
        { get { return _instance; } }

    private void Awake()
    {
        playercontroller = new PlayerController();

        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnEnable()
    {
        playercontroller.Enable();
    }

    private void OnDisable()
    {
        playercontroller.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playercontroller.Player.Movement.ReadValue<Vector2>();
    }
    public Vector2 GetMouseDelta()
    {
        return playercontroller.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumpedThisFrame()
    {
        return playercontroller.Player.Jump.triggered;
    }

    public bool PlayerCrouchedThisFrame()
    {
        return playercontroller.Player.Crouch.triggered;
    }

    public bool PlayerAttack()
    {
        return playercontroller.Player.Attack.triggered;
    }



}





