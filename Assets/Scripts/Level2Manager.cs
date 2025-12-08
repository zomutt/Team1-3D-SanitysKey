using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Manager : MonoBehaviour
{
    // --------- SINGLETON ---------
    public static Level2Manager Instance { get; private set; }

    // --------- SAVED DATA ---------
    bool hasTouched;

    public PlayerController PlayerController;
    public InventoryController InventoryController;

    int savedBulbs;
    int savedMedkits;
    int savedLaudanum;

    Vector3 checkpointPosition;
    Quaternion checkpointRotation;

    // assign this in the inspector
    public Transform checkpointSpawnPoint;

    // -----------------------------------------
    private void Awake()
    {
        // Basic Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        hasTouched = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // called after a scene reload
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerController = FindFirstObjectByType<PlayerController>();
        InventoryController = FindFirstObjectByType<InventoryController>();

        if (hasTouched)
        {
            ApplyCheckpoint();
        }
    }

    // ----------------------------------------------------------
    // ------------------- SAVE CHECKPOINT -----------------------
    // ----------------------------------------------------------

    public void SavePlayerStats()
    {
        if (InventoryController == null || PlayerController == null)
            return;

        hasTouched = true;

        savedBulbs = InventoryController.bulbCharges;
        savedMedkits = InventoryController.medCharges;
        savedLaudanum = InventoryController.laudanumCharges;

        checkpointPosition = checkpointSpawnPoint.position;
        checkpointRotation = checkpointSpawnPoint.rotation;

        Debug.Log("Checkpoint saved.");
    }


    // ----------------------------------------------------------
    // -------------------- RESET PLAYER -------------------------
    // ----------------------------------------------------------

    public void ResetPlayer()
    {
        // restore player stats
        PlayerController.pHP = PlayerController.pHPMax;
        PlayerController.pStam = PlayerController.pStamMax;
        PlayerController.pSanity = PlayerController.pSanityMax;

        // restore inventory
        InventoryController.bulbCharges = savedBulbs;
        InventoryController.medCharges = savedMedkits;
        InventoryController.laudanumCharges = savedLaudanum;

        PlayerController.hasKey1 = true;
        InventoryController.hasMatches = true;
        InventoryController.hasRose = true;
        InventoryController.hasCanteen = true;

        Debug.Log("Player stats restored from checkpoint.");
    }


    // ----------------------------------------------------------
    // ------------------- APPLY CHECKPOINT ----------------------
    // ----------------------------------------------------------

    public void ApplyCheckpoint()
    {
        if (PlayerController == null || InventoryController == null)
            return;

        ResetPlayer();

        // move player
        PlayerController.transform.position = checkpointPosition;
        PlayerController.transform.rotation = checkpointRotation;
    }
}
