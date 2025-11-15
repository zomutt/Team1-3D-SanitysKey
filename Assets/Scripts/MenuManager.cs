using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    PlayerController PlayerController;
    public GameObject FirstPanel;
    public GameObject ConfirmPanel;

    private void Awake()
    {
        FirstPanel.SetActive(true);
        ConfirmPanel.SetActive(false);
    }
    public void FirstQuitOnClick()
    {
        FirstPanel.SetActive(false);
        ConfirmPanel.SetActive(true);
    }

    public void PersistOnClick()
    {
        SceneManager.LoadScene(PlayerController.LevelOnDeath);
        if (PlayerController.LevelOnDeath == "Level1") { SceneManager.LoadScene("Level1"); }
        else if (PlayerController.LevelOnDeath == "Level2") { SceneManager.LoadScene("Level2"); }
        else { Debug.Log("Error fetching scene."); }
    }

    public void ConfirmQuitOnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
