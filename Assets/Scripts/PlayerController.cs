using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;   //allows you to customize input via script
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }        //adds singleton for easy reference esp between levels

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

    [Header("Health")]
    public int pHP = 100;
    public int pHPMax = 100;
    bool canDmg;
    public string LevelOnDeath;

    [Header("Sanity")]
    public float pSanity = 100;
    public float pSanityMax = 100;
    public float pSanityRegen = 1f;
    public bool sanityWarning;
    public bool sanityDanger;
    public bool sanityLost;

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
    bool isFlashOn; //needed for entity dispersement
    public bool hasKey1;
    public bool hasKey2;
    [SerializeField] private LayerMask interactLayerMask;

    [Header("Interaction UI")]
    Collider currentAimCollider;


    [Header("Sanity - Entities")]
    [SerializeField] string entityTag = "Entity";

    [SerializeField] float sanityLookDrainRate = 4f;   // /sec when staring
    [SerializeField] float sanityLookRange = 10f;  // raycast distance
    [SerializeField] float sanityNearDrainRate = .5f;   // /sec when near
    [SerializeField] float sanityNearRadius = 10f;  // sphere radius
    [HideInInspector] bool proximityMessageShown;   // tracks if we've already shown the "watching me" text
    [HideInInspector] bool facingEntityMessageShown;   // tracks if we've shown the "staring" line

    [Header("Misc.")]
    [HideInInspector] public bool hasCanteen;
    public GameObject fireParent;
    public int sconeCount;
    public GameObject particle1;
    public GameObject particle2;
    public GameObject particle3;
    public GameObject particle4;
    public GameObject lightDoor;
    public GhostController GhostController;
    public UIPanelsManager UIPanelsManager;
    bool deathMusPlayed;
    public GameObject PPV;
    bool ladderWarned;
    public Level2Manager Level2Manager;
    public bool inRosalinRange;
    public bool hasGivenRose;
    public FinalPuzzle FinalPuzzle;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip sizzle;
    public AudioClip fireSuccess;
    public AudioClip sanity1;
    public AudioClip sanity2;
    public AudioClip sanity3;
    public AudioClip nostam;
    public AudioClip norange;
    public AudioClip medpack;
    public AudioClip watersuccess;
    public AudioClip waterfilthy;
    public AudioClip watching;
    public AudioClip notinhead;
    public AudioClip locked;
    public AudioClip laudanum;
    public AudioClip key;
    public AudioClip hurt;
    public AudioClip firecurious;
    public AudioClip emptycanteen;
    public AudioClip bulb;
    public AudioClip walkingSFX;
    public AudioClip runningSFX;
    public AudioClip DeathSequence;
    void Awake()
    {
        FinalPuzzle = FindFirstObjectByType<FinalPuzzle>();
        InventoryController = FindFirstObjectByType<InventoryController>();
        Level2Manager = FindFirstObjectByType<Level2Manager>();
        audioSource = GetComponent<AudioSource>();
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;            //needed for other scripts to easily reference the player
        //DontDestroyOnLoad(gameObject);        //WE PERSIST .......................  nvm

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
        deathMusPlayed = false;
        canJump = true;
        canDmg = true;
        hasKey1 = false;
        hasCanteen = false;
        stamRegen = 5f;        //i really should not have to hard code it this way but unity keeps setting it to .2 for some godforsaken reason if i dont
        ladderWarned = false;

        characterController = GetComponent<CharacterController>();
        particle1.SetActive(false);
        particle2.SetActive(false);
        particle3.SetActive(false);
        particle4.SetActive(false);
        lightDoor.SetActive(false);

        //PPV.SetActive(true);
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

        LockMouse();      //locks mouse on start,, can be enabled/disabled via other scripts as needed

        sanityWarning = false;
        sanityDanger = false;
        sanityLost = false;
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

    void Update()
    {
        // --- Sanity warning messages ---

        //reset flags once we pass threshold
        if (pSanity > 40f)
        {
            // Back to safe-ish zone -> all stages can retrigger later
            sanityWarning = false;
            sanityDanger = false;
            sanityLost = false;
        }
        else if (pSanity > 20f)
        {
            // Above 20: danger + lost can retrigger later
            sanityDanger = false;
            sanityLost = false;
        }
        else if (pSanity > 5f)
        {
            // Above 5: "lost" can retrigger later
            sanityLost = false;
        }

        //fire messages when entering each band (only once per entry)

        if (pSanity <= 40f && pSanity > 20f && !sanityWarning)
        {
            // First stage: starting to slip
            FeedbackBanner.Instance.Show("What is... happening?");
            audioSource.PlayOneShot(sanity1);
            sanityWarning = true;      // mark as shown until we above 40
        }
        else if (pSanity <= 20f && pSanity > 5f && !sanityDanger)
        {
            // Second stage: serious danger
            FeedbackBanner.Instance.Show("It's taking over...");
            audioSource.PlayOneShot(sanity2);
            sanityDanger = true;       // mark as shown until above 20
        }
        else if (pSanity <= 5f && !sanityLost)
        {
            // Final stage: completely lost
            audioSource.PlayOneShot(sanity3);
            FeedbackBanner.Instance.Show("It's over...");
            sanityLost = true;         // mark as shown until above 5
        }


        if (pHP < 1 || pSanity < 1)       // GGEZ
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 1f;
            
            canMove = false;
            canJump = false;
            UIPanelsManager.DeathScene();     //opens panel for it
            if (!deathMusPlayed) { audioSource.PlayOneShot(DeathSequence); deathMusPlayed = true; Destroy(gameObject); }
        }


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
        if (!isRunning && canMove && runAction.WasPressedThisFrame() && pStam < 5f) FeedbackBanner.Instance.Show("I'm too tired for that.");

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
        {
            FeedbackBanner.Instance.Show("I'm too tired for that.");
            audioSource.PlayOneShot(nostam);
        }
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

        bool lookingAtEntity = UpdateSanityFacingEntity();   //checks to see if player is directly staring at entity (raycast from cam, returns true/false and handles drain
        bool nearEntity = UpdateSanityProximity();      //checks if any entity is within radius,, allows for slow drain creep

        // only regen if we are NOT looking at one and NOT near one
        canRegenSanity = !(lookingAtEntity || nearEntity);    //makes sure player can only regen sanity when safe

        if ((pSanity < pSanityMax) && canRegenSanity)
        {
            pSanity += pSanityRegen * Time.deltaTime;
            if (pSanity > pSanityMax) pSanity = pSanityMax;
        }

        UpdateAimTarget();   //make sure this stays at end of Update method
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
    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canMove = true;
        lookAction.Enable();
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        canMove = false;
        lookAction.Disable();
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
            FeedbackBanner.Instance.Show("I need to get closer.");
            return;
        }

        IInteractable interactable = currentAimCollider.GetComponentInParent<IInteractable>();     //your programmer leveled tf upppp, interfaces are neat and basically tell the code to fire WITHOUT the need for an endless if/else chain
        if (interactable != null)
        {
            interactable.Interact(this);
            return;    // stop here; the object has done its thing
        }

        //old logic//fallback code,, you cant make me optimize my code when we're this short on time
        GameObject targetObject = currentAimCollider.gameObject;

        if (targetObject.CompareTag("MedPack"))
        {
            Debug.Log("Picked up med pack via interact");
            InventoryController.medCharges++;
            FeedbackBanner.Instance.Show("Ah! A medpack. This seems useful.");
            targetObject.SetActive(false);
        }
        else if (targetObject.CompareTag("Lightbulb"))
        {
            Debug.Log("Picked up lightbulb via interact");
            InventoryController.bulbCharges++;
            FeedbackBanner.Instance.Show("Thank god, another flashlight bulb.");
            targetObject.SetActive(false);
        }
        else if (targetObject.CompareTag("Laudanum"))
        {
            Debug.Log("Picked up laudanum");
            InventoryController.laudanumCharges++;
            FeedbackBanner.Instance.Show("What a blessing, this should help me keep it together in here.");
            targetObject.SetActive(false);
        }
        else if (targetObject.CompareTag("Key1"))
        {
            FeedbackBanner.Instance.Show("I wonder what this may be to.");
            hasKey1 = true;
            audioSource.PlayOneShot(key);
            targetObject.SetActive(false);
        }
        else if (targetObject.CompareTag("Door"))
        {
            if (hasKey1)
            {
                DoorController door = currentAimCollider.GetComponentInParent<DoorController>();

                if (door != null)
                {
                    door.OpenDoor();
                }
            }
            else { FeedbackBanner.Instance.Show("It's locked."); audioSource.PlayOneShot(locked); }
        }

        else if (targetObject.CompareTag("Door2"))
        {
            if (hasKey2)
            {
                DoorController door = currentAimCollider.GetComponentInParent<DoorController>();

                if (door != null)
                {
                    door.OpenDoor();
                }
            }
            else { FeedbackBanner.Instance.Show("It's locked."); audioSource.PlayOneShot(locked); }
        }

        else if (targetObject.CompareTag("Key2"))
        {
            FeedbackBanner.Instance.Show("This must lead to one of the bedrooms.");
            hasKey2 = true;
            targetObject.SetActive(false);
        }

        else if (targetObject.CompareTag("Canteen"))
        {
            targetObject.SetActive(false);
            hasCanteen = true;
            InventoryController.hasCanteen = true;
            InventoryController.canteenCount++;
            FeedbackBanner.Instance.Show("If only this had some water in it.");
        }
        else if (targetObject.CompareTag("Fire"))
        {
            Debug.Log("Fire interacted.");
            if (InventoryController.canteenCount == 0) { FeedbackBanner.Instance.Show("That's odd, I thought this place was abandoned. Could this be the key to something?"); audioSource.PlayOneShot(firecurious); }
        }

        else if (targetObject.CompareTag("Water"))
        {
            Debug.Log("Water interacted with.");
            if (!hasCanteen) { FeedbackBanner.Instance.Show("Ugh, this water is filthy!"); }
            else if (hasCanteen)
            {
                InventoryController.filledCanteenCount++;
                FeedbackBanner.Instance.Show("Ugh, this water is filthy! Still, I'll fill my canteen with it.");
            }
        }
        else if (targetObject.CompareTag("Matchbox"))
        {
            Debug.Log("Matchbox");
            InventoryController.hasMatches = true;
            FeedbackBanner.Instance.Show("Some matches... This may come in handy.");
        }

        else if (targetObject.CompareTag("Rose"))
        {
            Debug.Log("Rose interacted");
            if (!GhostController.ghostAwakened) { FeedbackBanner.Instance.Show("These roses are absolutely beautiful. Let's respect the dead and leave them be, lest we anger the deceased."); }
            else
            {
                FeedbackBanner.Instance.Show("Some beautiful roses are growing on this grave... This seems to be what Briar Rose has been looking for -- OW! That's sharp.");
                pHP -= 10;
                targetObject.SetActive(false);
                InventoryController.hasRose = true;
                InventoryController.roseImg.SetActive(true);
            }
        }
        else if (targetObject.CompareTag("Ladder"))
        {
            if (!ladderWarned)
            { FeedbackBanner.Instance.Show("I can climb this ladder, but I might not be able to return."); ladderWarned = true; }
            else if (ladderWarned)
            {
                Level2Manager.SavePlayerStats();
                Level2Manager.ApplyCheckpoint();
            }
        }
        else if (targetObject.CompareTag("CatToy"))
        {
            FeedbackBanner.Instance.Show("A toy mouse... How cute.");
            InventoryController.hasToy = true;
            targetObject.SetActive(false);
        }
        else if (targetObject.CompareTag("Kitty"))
        { 
            FeedbackBanner.Instance.Show("*You pet the cat, it purrs happily.*");
        }
        else if (targetObject.CompareTag("Rosalin"))
        {
            if (!hasGivenRose) MiscFeedbackBanner.Instance.Show("I sense you have something of mine?");
            else if (hasGivenRose) MiscFeedbackBanner.Instance.Show("Thank you for bringing me my rose, you must have met my daughter... You should leave, take this pendant and key by the door and go.");
        }
        else if (targetObject.CompareTag("AltarBook"))
        {
            FinalPuzzle.OpenAltarBook();
        }
    }
    public void TakeDmg()
    {
        if (canDmg)
        {
            pHP -= dmgToPC;
            FeedbackBanner.Instance.Show("Ah! I'm hurt!");
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

    // Returns true if we are currently looking at an Entity
    bool UpdateSanityFacingEntity()
    {
        bool isLookingAtEntity = false;

        // Ray from camera forward
        Ray ray = new Ray(playerCamera.transform.position,
                          playerCamera.transform.forward);
        RaycastHit hit;

        // Shoot ray up to sanityLookRange units
        if (Physics.Raycast(ray, out hit, sanityLookRange))
        {
            // Many models put the collider on a CHILD object while the tag
            // is on the PARENT, so we check the top-most parent (root).
            Transform rootTransform = hit.collider.transform.root;

            if (rootTransform.CompareTag(entityTag))
            {
                isLookingAtEntity = true;
            }
        }

        // If we actually confirmed we're looking at an Entity, drain sanity
        if (isLookingAtEntity)
        {
            pSanity -= sanityLookDrainRate * Time.deltaTime;
            if (pSanity < 0f) pSanity = 0f;

            // Show this line once when we first start staring at it
            if (!facingEntityMessageShown)
            {
                FeedbackBanner.Instance.Show("No way that's *not* just in my head... I wish it was, though.");
                audioSource.PlayOneShot(notinhead);
                facingEntityMessageShown = true;
            }
        }
        else
        {
            // Not looking at an Entity anymore: allow the line to trigger next time
            facingEntityMessageShown = false;
        }

        return isLookingAtEntity;
    }



    // Returns true if any Entity is within sanityNearRadius
    bool UpdateSanityProximity()
    {
        bool isNearEntity = false;

        // Check for any colliders in radius
        Collider[] hits = Physics.OverlapSphere(transform.position, sanityNearRadius);
        for (int index = 0; index < hits.Length; index++)
        {
            // If tag is on the parent, use hits[index].transform.root.CompareTag(entityTag)
            if (hits[index].CompareTag(entityTag))
            {
                isNearEntity = true;
                break;
            }
        }

        if (isNearEntity)
        {
            // slow sanity drain
            pSanity -= 0.5f * Time.deltaTime;
            if (pSanity < 0f) pSanity = 0f;

            // only show the message ONCE while we're in range
            if (!proximityMessageShown)
            {
                FeedbackBanner.Instance.Show("Is something... watching me?");
                audioSource.PlayOneShot(watching);
                proximityMessageShown = true;
            }
        }
        else
        {
            // out of range: allow it to fire again next time we get close
            proximityMessageShown = false;
        }

        return isNearEntity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TubRange")) { InventoryController.inTubRange = true; Debug.Log("In range: " + InventoryController.inTubRange); }
        if (other.CompareTag("FireRange")) { InventoryController.inRangeCanteen = true; Debug.Log("In range: " + InventoryController.inRangeCanteen); }
        if (other.CompareTag("KittyRange")) { InventoryController.inCatRange = true; Debug.Log("In range: " + InventoryController.inCatRange); }
        if (other.CompareTag("RosalinRange")) { inRosalinRange = true; Debug.Log("In Rosalin range: " + inRosalinRange); }
        if (other.CompareTag("AltarRange")) { FinalPuzzle.inAltarRange = true; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TubRange")) { InventoryController.inTubRange = false; Debug.Log("In range: " + InventoryController.inTubRange); }
        if (other.CompareTag("FireRange")) { InventoryController.inRangeCanteen = false; Debug.Log("In range: " + InventoryController.inRangeCanteen); }
        if (other.CompareTag("KittyRange")) { InventoryController.inCatRange = false; Debug.Log("In range: " + InventoryController.inCatRange); }
        if (other.CompareTag("RosalinRange")) { inRosalinRange = false; Debug.Log("In Rosalin range: " + inRosalinRange); }
        if (other.CompareTag("AltarRange")) { FinalPuzzle.inAltarRange = false; }
    }

    public void LightCandle()
    {
        if (!InventoryController.hasMatches)
        { FeedbackBanner.Instance.Show("I don't have any way to light this."); return; }

        if (currentAimCollider == null) { FeedbackBanner.Instance.Show("I need something to light first."); return;}

        float distanceToTarget = Vector3.Distance(playerCamera.transform.position, currentAimCollider.transform.position); //DISTANCE CHECK

        if (distanceToTarget > interactRange) 
        { 
            FeedbackBanner.Instance.Show("I need to get closer."); 
            audioSource.PlayOneShot(norange); 
            return; 
        }

        Candles candle = currentAimCollider.GetComponentInParent<Candles>();       //this checks for script on object or one of its parents
        if (candle == null)
        {
            FeedbackBanner.Instance.Show("That won't help me here.");
            return;
        }

        candle.LightCandle();
    }
}

