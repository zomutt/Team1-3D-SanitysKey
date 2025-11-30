using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    public GameObject HelpPanel;
    public GameObject ObjPanel;
    public GameObject ControlsPanel;
    public GameObject SurvivalPanel;
    int panelChoice;

    private void Awake()
    {
        panelChoice = -1;
        HelpPanel.SetActive(false);
        ObjPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        SurvivalPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (panelChoice)
            {
                case 0:
                    HelpPanel.SetActive(false);
                    break;
                case 1:
                    ObjPanel.SetActive(false);
                    break;
                case 2:
                    ControlsPanel.SetActive(false);
                    break;
                case 3:
                    SurvivalPanel.SetActive(false);
                    break;
            }
        }
    }
    public void OnClickPlay() { SceneManager.LoadScene("Level1"); }
    public void OnClickHelp() { Debug.Log("Help clicked"); HelpPanel.SetActive(true); panelChoice = 0;  }
    public void OnClickQuit() { Application.Quit(); }
    public void OnClickObjective() {  ObjPanel.SetActive(true); panelChoice = 1; }
    public void OnClickControls() {  ControlsPanel.SetActive(true); panelChoice = 2; }
    public void OnClickSurvival() { SurvivalPanel.SetActive(true); panelChoice= 3; }
    public void OnClickReturn() { HelpPanel.SetActive(false); }
}
