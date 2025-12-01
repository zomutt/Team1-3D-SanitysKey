using UnityEngine;
using UnityEngine.SceneManagement;

public class InvAcrossScenes : MonoBehaviour
{
    public int storedAid;
    public int storedBulb;
    public int storedLaudanum;
    bool hasLoaded;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        hasLoaded = false;
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Level2")
        {

        }
    }
}
