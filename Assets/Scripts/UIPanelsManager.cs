using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPanelsManager : MonoBehaviour
{
    public GameObject FirstDeathPanel;
    public GameObject DeathConfirmPanel;
    public GameObject DeathScenePanel;
    public GameObject HelpPanelParent;
    public GameObject ControlsPanel;
    public GameObject ObjectivesPanel;
    public GameObject SurvivalPanel;
    public GameObject MainEscPanel;
    public GameObject ConfirmEscPanel;
    public GameObject EscapeText;
    public PlayerController PlayerController;
    public GameObject player;
    bool isDead;
    bool isEscOpen;
    bool confirmOpen;

    private void Awake()
    {
        DeathScenePanel.SetActive(false);
        HelpPanelParent.SetActive(false);
        isDead = false;
        EscapeText.SetActive(true);
    }

    public void DeathScene()
    {
        DeathScenePanel.SetActive(true);
        FirstDeathPanel.SetActive(true);
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if (Input.GetKeyDown(KeyCode.M))        //madness,, pulls up confirm box
            {
                FirstDeathPanel.SetActive(false);
                DeathConfirmPanel.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.U))      //quit to mm
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
        if (!isDead)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isEscOpen)
            {
                Debug.Log("Escape registered by DSM, turning on help panel");
                HelpPanelParent.SetActive(true);
                isEscOpen = true;
                PlayerController.canMove = false;
                ConfirmEscPanel.SetActive(false);
                MainEscPanel.SetActive(true);
                ControlsPanel.SetActive(true);
                ObjectivesPanel.SetActive(true);
                SurvivalPanel.SetActive(true);
                EscapeText.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && isEscOpen)     //close
            {
                HelpPanelParent.SetActive(false);
                isEscOpen = false;
                PlayerController.canMove = true;
                EscapeText.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))        //turns on controlls
        {
            MainEscPanel.SetActive(false);            
            ControlsPanel.SetActive(true);
            ObjectivesPanel.SetActive(false);
            SurvivalPanel.SetActive(false);
            ConfirmEscPanel.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.F2))      //turns on objectives
        {
            MainEscPanel.SetActive(false);
            ControlsPanel.SetActive(false);
            ObjectivesPanel.SetActive(true);
            SurvivalPanel.SetActive(false);
            ConfirmEscPanel.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.F3))         //turns on survival tips
        {
            MainEscPanel.SetActive(false);
            ControlsPanel.SetActive(false);
            ObjectivesPanel.SetActive(false);
            SurvivalPanel.SetActive(true);
            ConfirmEscPanel.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.F4))       //quit w confirmation
        {
            if (!confirmOpen) { ConfirmEscPanel.SetActive(true); confirmOpen = true; }
            else if (confirmOpen) { Debug.Log("Quit confirmed"); SceneManager.LoadScene("MainMenu"); Destroy(player); }
        }

    }
}
