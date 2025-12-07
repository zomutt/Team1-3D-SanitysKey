using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathSceneManager : MonoBehaviour
{
    public GameObject FirstPanel;
    public GameObject DeathConfirmPanel;
    public GameObject DeathScenePanel;
    public GameObject MainEscPanelParent;
    public GameObject ControlsPanel;
    public GameObject ObjectivesPanel;
    public GameObject SurvivalPanel;
    public GameObject FirstEscPanel;
    public GameObject ConfirmEscPanel;
    public GameObject EscapeText;
    public PlayerController PlayerController;
    public GameObject PLAYER;
    bool isDead;
    bool isEscOpen;
    bool confirmOpen;

    private void Awake()
    {
        DeathScenePanel.SetActive(false);
        MainEscPanelParent.SetActive(false);
        isDead = false;
        EscapeText.SetActive(true);
    }

    public void DeathScene()
    {
        DeathScenePanel.SetActive(true);
        FirstPanel.SetActive(true);
        DeathConfirmPanel.SetActive(false);
        ConfirmEscPanel.SetActive(false);
        isDead = true;
        confirmOpen = false;
    }

    private void Update()
    {
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.P))       //restart,, will add to later for savepoints when possible
            {
                SceneManager.LoadScene("Level1");
                isDead = false;   //not needed now, is needed later
            }
            if (Input.GetKeyDown(KeyCode.M))        //madness,, pulls up confirm box
            {
                FirstPanel.SetActive(false);
                DeathConfirmPanel.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.U))      //quit to mm
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
        if (!isDead)
        {
            if (!isDead)
            {
                if (Input.GetKeyDown(KeyCode.Escape) && !isEscOpen)
                {
                    Debug.Log("Escape registered by DSM, turning on help panel");
                    MainEscPanelParent.SetActive(true);
                    isEscOpen = true;
                    PlayerController.canMove = false;
                    ConfirmEscPanel.SetActive(false);
                    FirstEscPanel.SetActive(true);
                    ControlsPanel.SetActive(true);
                    ObjectivesPanel.SetActive(true);
                    SurvivalPanel.SetActive(true);
                    EscapeText.SetActive(false);
                }
                else if (Input.GetKeyDown(KeyCode.Escape) && isEscOpen)
                {
                    MainEscPanelParent.SetActive(false);
                    isEscOpen = false;
                    PlayerController.canMove = true;
                    EscapeText.SetActive(true);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            FirstEscPanel.SetActive(false);            
            ControlsPanel.SetActive(true);
            ObjectivesPanel.SetActive(false);
            SurvivalPanel.SetActive(false);
            ConfirmEscPanel.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            FirstEscPanel.SetActive(false);
            ControlsPanel.SetActive(false);
            ObjectivesPanel.SetActive(true);
            SurvivalPanel.SetActive(false);
            ConfirmEscPanel.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            FirstEscPanel.SetActive(false);
            ControlsPanel.SetActive(false);
            ObjectivesPanel.SetActive(false);
            SurvivalPanel.SetActive(true);
            ConfirmEscPanel.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            if (!confirmOpen) { ConfirmEscPanel.SetActive(true); confirmOpen = true; }
            else if (confirmOpen) { Destroy(PLAYER); SceneManager.LoadScene("MainMenu");  }
        }

    }
}
