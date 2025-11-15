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
    public InventoryController InventoryController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0f;
    [HideInInspector] public bool canMove = true;

    [Header("Damage")]
    [SerializeField] HurtOverlay HurtOverlay;   

    //Input Actions (created in code, no PlayerInput needed)
    [Header("InputActions")]
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction runAction;
    InputAction interactAction;

    [Header("Stats")]
    public int pHP = 100;
    public int pHPMax = 100;
    public float pSanity = 100;
    public float pSanityMax = 100;
    public float pSanityRegen = 2;
    bool canDmg;

    [Header("Stamina")]     //these can all be tweaked as needed in inspector
    public float pStam = 100;
    public float pStamMax = 100;
    public float runCost = 5f;   //stam points per sec while sprinting ( stam * Time.deltaTime )
    public float jumpCost = 10f;      //stam cost to jump
    public float stamRegen = 2f;      //how much stam is regen'd /sec,, its not actually changed by this value. unity just likes to smoke crack. idfk.
    bool canJump;
    [HideInInspector] public int dmgToPC;
    [HideInInspector] public bool canRegenSanity;    //stops player from getting sanity regen while actively facing horrors

    [Header("Interaction")]
    [SerializeField] float interactRange = 3f;
    [SerializeField] string[] interactableTags;          // tags that count as interactable
    [SerializeField] CrosshairController CrosshairController;

    [Header("Interaction UI")]
    Collider currentAimCollider;
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
        // Interact: left mouse button (*MIGHT* swap to E later, tbd)
        interactAction = new InputAction("Interact");
        interactAction.AddBinding("<Mouse>/rightButton");

        pHP = pHPMax;         //makes sure she's at full stats upon awake
        pStam = pStamMax;
        pSanity = pSanityMax;
        canJump = true;
        canDmg = true;

        stamRegen = 5f;        //i really should not have to hard code it this way but unity keeps setting it to .2 for some godforsaken reason if i dont

        characterController = GetComponent<CharacterController>();
    }
        void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        runAction.Enable();
        interactAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        runAction.Disable();
        interactAction.Disable();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    bool IsInteractableTag(string tagToCheck)
    {
        for (int index = 0; index < interactableTags.Length; index++)
        {
            if (tagToCheck == interactableTags[index])
            {
                return true;
            }
        }
        return false;
    }

    void Update()           //half of this movement controller was pulled off google then modified fyi
    {

        if (pStam < 1) pStam = 0;       //makes sure no neg stam
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


        UpdateAimTarget();   //make sure this stays at end of Update method
        UpdateAimTarget();
        HandleInteraction();

    }

    void UpdateAimTarget()           //i found this online so i'm not like.... super... sure how it works, but it feels p straightforward and it works
    {
        if (playerCamera == null) return;

        Ray cameraRay = new Ray(playerCamera.transform.position,
                                playerCamera.transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(cameraRay, out hitInfo, interactRange))
        {
            string hitTag = hitInfo.collider.tag;

            if (IsInteractableTag(hitTag))
            {
                if (currentAimCollider != hitInfo.collider)
                {
                    currentAimCollider = hitInfo.collider;
                    if (CrosshairController != null)
                    {
                        CrosshairController.SetState(CrosshairState.Interactable);
                    }
                }
                return; // still aiming at valid thing
            }
        }

        // if we get here, we are NOT aiming at an interactable
        if (currentAimCollider != null)
        {
            currentAimCollider = null;
            if (CrosshairController != null)
            {
                CrosshairController.SetState(CrosshairState.Default);
            }
        }
    }
    void HandleInteraction()
    {
        if (!interactAction.WasPressedThisFrame())
            return;

        if (currentAimCollider == null)
            return;

        // Distance check so you can highlight from farther away
        float distanceToTarget = Vector3.Distance(
            playerCamera.transform.position,
            currentAimCollider.transform.position);

        if (distanceToTarget > interactRange)
        {
            FeedbackBanner.Instance.Show("It's too far away.");
            return;
        }

        GameObject targetObject = currentAimCollider.gameObject;

        if (targetObject.CompareTag("MedPack"))
        {
            Debug.Log("Picked up med pack via interact");
            InventoryController.medCharges++;
            FeedbackBanner.Instance.Show("This seems useful.");
            targetObject.SetActive(false);
        }
        else if (targetObject.CompareTag("Lightbulb"))
        {
            Debug.Log("Picked up lightbulb via interact");
            InventoryController.bulbCount++;
            FeedbackBanner.Instance.Show("This might help my flashlight.");
            targetObject.SetActive(false);
        }
    }


    public void TakeDmg()
    {
        if (canDmg)
        {
            pHP -= dmgToPC;
            FeedbackBanner.Instance.Show("I'm hurt!");
            Debug.Log("Player hp: " + pHP);
            StartCoroutine(iframe());
            HurtOverlay.Play();
        }
        else { return; }
    }

    IEnumerator iframe()
    {
        canDmg = false;
        yield return new WaitForSeconds(.75f);
        canDmg = true;
    }
}
