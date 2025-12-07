using UnityEngine;

public class SconeManager : MonoBehaviour
{
    public int sconeCount;
    public string sconeString;
    PlayerController PlayerController;
    public GameObject flame1, flame2, flame3, flame4;
    public GameObject sconeDoor;
    bool hasCompleted;
    void Start()
    {
        sconeCount = 0;
        sconeString = "";
        flame1.SetActive(false); flame2.SetActive(false); flame3.SetActive(false); flame4.SetActive(false);
        sconeDoor.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasCompleted && sconeCount >= 3 && sconeString != "143") 
        { 
            flame1.SetActive(false); 
            flame2.SetActive(false); 
            flame3.SetActive(false); 
            flame4.SetActive(false); 
            sconeString = "";
            sconeCount = 0;
            FeedbackBanner.Instance.Show("Hm... Maybe let's try a different order. There may be a clue around here somewhere."); 
        }
        if (sconeString == "143") 
        { 
            flame2.SetActive(true);
            FeedbackBanner.Instance.Show("That did the trick! Let's go through this door.");
            sconeDoor.SetActive(false);
            sconeString = "";      //prevents endless spam of FB text
            hasCompleted = true;
        }
    }
}
