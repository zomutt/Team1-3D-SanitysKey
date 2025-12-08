using UnityEngine;

public class RosalinPuzzle : MonoBehaviour
{
    public GameObject pendant;
    public GameObject keyStudy;
    public bool catPlayed;
    public Material rosalinAbsent;
    public Material rosalinPresent;
    public Renderer rosalinRenderer;     //the renderer component of Rosalin's model
    public bool isRosalinPresent;
    public bool hasGivenRose;
    public PlayerController PlayerController;

    public float roseUseRange = 2f;   // how close the player must be to use 7 ((the rose))
    void Start()
    {
        PlayerController = FindFirstObjectByType<PlayerController>();    
        catPlayed = false;
        isRosalinPresent = false;
        pendant.SetActive(false);
        keyStudy.SetActive(false);

        if (rosalinRenderer != null && rosalinAbsent != null)      //defaults to absent material
        {
            rosalinRenderer.material = rosalinAbsent;
        }
    }

    void Update()
    {
        Vector3 toPlayer = PlayerController.transform.position - transform.position;        //this is our distance check,, i need to grow up and stop using invis cubes for everything
        toPlayer.y = 0f;                               // ignore vertical difference
        float distanceToPlayer = toPlayer.magnitude;
        bool inRange = distanceToPlayer <= roseUseRange;

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (inRange)
            {
                FeedbackBanner.Instance.Show("Rosalin seems pleased with the rose.");
                MiscFeedbackBanner.Instance.Show("Thank you for bringing me my rose, you must have met my daughter... You should leave, take the pendant and key by the door and go.");
                ActivateGifts();
            }
        }
    }
    public void ActivateRosalin()     //called by catwalk script when cat moves toward rosalin
    {
        isRosalinPresent = true;
        if (rosalinRenderer != null && rosalinPresent != null)
        {
            rosalinRenderer.material = rosalinPresent;
        }
    }

    void ActivateGifts()
    {
        if (!hasGivenRose)
        { 
            pendant.SetActive(true);
            keyStudy.SetActive(true);
            hasGivenRose = true; 
        }
    }
}
