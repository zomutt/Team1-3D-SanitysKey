using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;   //allows you to customize input via script

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7.5f;
    public float sprintSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    [Header("Camera")]
    public Camera playerCamera;
    public float lookSpeed = 2.0f;     // sensitivity scaler
    public float lookXLimit = 45.0f;   // clamp vertical look

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0f;
    [HideInInspector] public bool canMove = true;

    //Input Actions (created in code, no PlayerInput needed)
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction runAction;


    [Header("Stats")]
    public int pHP = 100;
    public int pHPMax = 100;
    public int pSanity = 100;
    public int pSanityMax = 100;

    [Header("Stamina")]     //these can all be tweaked as needed in inspector
    public float pStam = 100;
    public float pStamMax = 100;
    public float runCost = 5f;   //stam points per sec while sprinting ( stam * Time.deltaTime )
    public float jumpCost = 10f;      //stam cost to jump
    public float regenRate = 10f;         //stam in sec (after delay, rR * Time.deltaTime)
    public float regenDelay = 0.75f;  //how long it takes for stam to begin regennin
    public float minRun = 5f;    //min stam to run,, cant if empty
    public float minJump = 5f;   //same as above
    float lastUse;  // last time stamina was spent
    bool isRunning;
    bool canJump;
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

        pHP = pHPMax;         //makes sure she's at full stats upon awake
        pStam = pStamMax;
        pSanity = pSanityMax;
        canJump = true;
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

        if (pStam < 0) pStam = 0;
        // 1) Ground check first
        bool grounded = characterController.isGrounded;

        // 2) Read inputs (new Input System)
        Vector2 moveInput = canMove ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        Vector2 lookInput = canMove ? lookAction.ReadValue<Vector2>() : Vector2.zero;
        bool isRunning = canMove && runAction.IsPressed();

        // 3) Build world-space planar velocity,, checks for run,, also do not ask me what this means all ik is that it works
        float speed = isRunning ? sprintSpeed : moveSpeed;    //This uses the ternary operator. If isRunning is true, speed = sprintSpeed; otherwise speed = moveSpeed.
        Vector3 fwd = transform.forward;                //A unit direction vector that points where the player object is facing in world space (its “forward”).
        Vector3 right = transform.right;      //A unit direction vector that points to the player’s right in world space.

        float vy = moveDirection.y; // preserve vertical
        moveDirection = (fwd * moveInput.y + right * moveInput.x) * speed;

        // 4) Jump (press once while grounded)
        if (grounded && canMove && jumpAction.WasPressedThisFrame() && canJump)
        {
            moveDirection.y = jumpSpeed;
            pStam -= jumpCost;
        }
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

        if (isRunning && pStam > 0f)                   // same outer guard
        {
            // use the new Input System, not Input.GetKey
            if (runAction.IsPressed())                            // CHANGED
            {
                pStam -= runCost * Time.deltaTime;
                if (pStam < 0f) pStam = 0f;
            }
        }
    }
}



