using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;   //allows you to customize input via script

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }        //adds singleton for easy reference esp between levels,, NOTE: ONLY works if there is only one player

    [Header("Movement")]
    public float moveSpeed = 5.5f;
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
    public float pSanity = 100;
    public float pSanityMax = 100;
    public float pSanityRegen = 2;

    [Header("Stamina")]     //these can all be tweaked as needed in inspector
    public float pStam = 100;
    public float pStamMax = 100;
    public float runCost = 5f;   //stam points per sec while sprinting ( stam * Time.deltaTime )
    public float jumpCost = 10f;      //stam cost to jump
    public float stamRegen = 2f;      //how much stam is regen'd /sec,, its not actually changed by this value. unity just likes to smoke crack. idfk.
    bool canJump;
    [HideInInspector] public int dmgToPC;
    [HideInInspector] public bool canRegenSanity;    //stops player from getting sanity regen while actively facing horrors

    
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;            //needed for other scripts to easily reference the player

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

        stamRegen = 5f;        //i really should not have to hard code it this way but unity keeps setting it to .2 for some godforsaken reason if i dont
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

    void Update()           //half of this movement controller was pulled off google then modified fyi
    {

        if (pStam < 0) pStam = 0;       //makes sure no neg stam
        if (pStam > pStamMax) pStam = pStamMax;     //samesies logic
        if (pHP > pHPMax) pHP = pHPMax;
        if (pSanity > pSanityMax) pSanity = pSanityMax;
        // 1) Ground check first
        bool grounded = characterController.isGrounded;

        // 2) Read inputs (new Input System)
        Vector2 moveInput = canMove ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        Vector2 lookInput = canMove ? lookAction.ReadValue<Vector2>() : Vector2.zero;
        bool isRunning = canMove && runAction.IsPressed() && pStam >= 5;    //RUNTIMEBOIS,, checks these conditions to see if player can run
        if (!isRunning && canMove && runAction.WasPressedThisFrame() && pStam < 5f) FeedbackBanner.Instance.Show("I'm too tired to run.");

        // 3) Build world-space planar velocity,, checks for run,, also do not ask me what this means all ik is that it works
        float speed = isRunning ? sprintSpeed : moveSpeed;    //This uses the ternary operator. If isRunning is true, speed = sprintSpeed; otherwise speed = moveSpeed.
        Vector3 fwd = transform.forward;                //A unit direction vector that points where the player object is facing in world space (its “forward”).
        Vector3 right = transform.right;      //A unit direction vector that points to the player’s right in world space.

        float vy = moveDirection.y; // preserve vertical
        moveDirection = (fwd * moveInput.y + right * moveInput.x) * speed;

        // 4) Jump (press once while grounded)
        if (grounded && canMove && jumpAction.WasPressedThisFrame() && canJump && (pStam >= 10))       //jump checks 
        {
            moveDirection.y = jumpSpeed;
            pStam -= jumpCost;
        }
        else
        {
            moveDirection.y = vy;
        }

        if (grounded && canMove && jumpAction.WasPressedThisFrame() && pStam < 10f)
            FeedbackBanner.Instance.Show("I'm too tired to jump.");

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
            float yawDelta = lookInput.x * lookSpeed;
            float pitchDelta = -lookInput.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX + pitchDelta, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            transform.rotation *= Quaternion.Euler(0f, yawDelta, 0f);
        }

        if (isRunning)         //draim stam :3
        {
            pStam -= runCost * Time.deltaTime;
            if (pStam < 0f) pStam = 0f;
        }

        if (!isRunning && grounded && (pStam < pStamMax) && (speed == moveSpeed))     //checks these conditions before running regen
        {
            pStam += stamRegen * Time.deltaTime;   //sets stam regen /sec
        }

        if ((pSanity < pSanityMax) && canRegenSanity)     //checks these conditions before running regen
        {
            pSanity += pSanityRegen * Time.deltaTime;   //sets stam regen /sec
        }
    }

    public void TakeDmg()    //ksobasically,, other scripts call this method BUT then feed in its own dmg values and such if things actually like..... work right. which they aren't. might just end up being for visuals.
    {
        pHP -= dmgToPC;
        Debug.Log("Player hp: " + pHP);
    }


    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Lightbulb"))
    //    {
    //        InventoryController.flashLightCharges++;
    //        Destroy(other.gameObject);
    //    }
    //}
}





