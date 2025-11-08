using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;   //allows you to customize input via script

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Movement
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    // Look
    public Camera playerCamera;
    public float lookSpeed = 2.0f;     // sensitivity scaler
    public float lookXLimit = 45.0f;   // clamp vertical look

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0f;
    [HideInInspector] public bool canMove = true;

    // NEW: Input Actions (created in code, no PlayerInput needed)
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction runAction;

    void Awake()
    {
        // Move: WASD + gamepad left stick
        moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // Look: mouse delta + gamepad right stick
        lookAction = new InputAction("Look");
        lookAction.AddBinding("<Mouse>/delta");
        lookAction.AddBinding("<Gamepad>/rightStick");

        // Jump / Run
        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
        runAction = new InputAction("Run", binding: "<Keyboard>/leftShift");
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        runAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        runAction.Disable();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1) Ground check first
        bool grounded = characterController.isGrounded;

        // 2) Read inputs (new Input System)
        Vector2 moveInput = canMove ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        Vector2 lookInput = canMove ? lookAction.ReadValue<Vector2>() : Vector2.zero;
        bool isRunning = canMove && runAction.IsPressed();

        // 3) Build world-space planar velocity
        float speed = isRunning ? runningSpeed : walkingSpeed;
        Vector3 fwd = transform.forward;
        Vector3 right = transform.right;

        float vy = moveDirection.y; // preserve vertical
        moveDirection = (fwd * moveInput.y + right * moveInput.x) * speed;

        // 4) Jump (press once while grounded)
        if (grounded && canMove && jumpAction.WasPressedThisFrame())
            moveDirection.y = jumpSpeed;
        else
            moveDirection.y = vy;

        // Optional: keep controller “stuck” to ground/slopes
        if (grounded && moveDirection.y < 0f)
            moveDirection.y = -2f;

        // 5) Gravity
        if (!grounded)
            moveDirection.y -= gravity * Time.deltaTime;

        // 6) Move
        characterController.Move(moveDirection * Time.deltaTime);

        // 7) Mouse/Gamepad look (pitch on camera, yaw on body)
        if (canMove)
        {
            // Note: mouse delta is already per-frame; no Time.deltaTime here
            float yawDelta = lookInput.x * lookSpeed;
            float pitchDelta = -lookInput.y * lookSpeed;

            rotationX = Mathf.Clamp(rotationX + pitchDelta, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            transform.rotation *= Quaternion.Euler(0f, yawDelta, 0f);
        }
    }
}
