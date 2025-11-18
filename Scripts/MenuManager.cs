using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject FirstPanel;
    public GameObject ConfirmPanel;

    private void Awake()
    {
        FirstPanel.SetActive(true);
        ConfirmPanel.SetActive(false);
    }
    void FirstQuitOnClick()
    {
        FirstPanel.SetActive(false);
        ConfirmPanel.SetActive(true);
    }

    void PersistOnClick()
    {
        SceneManager.LoadScene("Level1");
    }

    void ConfirmQuitOnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
