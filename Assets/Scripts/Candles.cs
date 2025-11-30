using UnityEngine;

public class Candles : MonoBehaviour
{
    [SerializeField] GameObject fireFX;
    public bool isLit;
    public GhostController GhostController;
    public SconeManager SconeManager;
    void Awake()
    {
        // Auto-assign the flame if not set in the Inspector
        if (fireFX == null)
        {
            Transform flameChild = transform.Find("Flame");          //child name

            if (flameChild != null)  { fireFX = flameChild.gameObject; }
            else { Debug.LogWarning("Candle Script on " + name + " could not find child 'Flame'."); }
        }

        if (fireFX != null) { fireFX.SetActive(false); } 
    }
    public void LightCandle()
    //{
    //    if (isLit) { return; }

    //    isLit = true;
    { 
        if (gameObject.name == "Candle") { GhostController.candlesLit++; }
        if (gameObject.name == "scone1") { SconeManager.sconeString += "1"; SconeManager.sconeCount++; }
        if (gameObject.name == "scone2") { SconeManager.sconeString += "2"; SconeManager.sconeCount++; }
        if (gameObject.name == "scone3") { SconeManager.sconeString += "3"; SconeManager.sconeCount++; }
        if (gameObject.name == "scone4") { SconeManager.sconeString += "4"; SconeManager.sconeCount++; }

        if (fireFX != null) { fireFX.SetActive(true); }
        
        else { Debug.LogWarning("Candle Script: fireFX is not assigned on " + name); }
    }
}
