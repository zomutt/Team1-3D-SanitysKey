using UnityEngine;

public class FireRange : MonoBehaviour
{
    public InventoryController InventoryController;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) { InventoryController.inRangeCanteen = true; Debug.Log("In range: " + InventoryController.inRangeCanteen); }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { InventoryController.inRangeCanteen = false; Debug.Log("In range: " + InventoryController.inRangeCanteen); }
    }
}
