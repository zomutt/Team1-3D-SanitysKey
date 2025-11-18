using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    private PlayerController PlayerController;
    
    bool canDealDmg;
    public float dmgCooldown = 1.5f;
    public int dmgGiven = 10;

    void Start()
    {
        var pc = PlayerController.Instance;           
        if (pc == null) { Debug.LogError("No PlayerController instance."); return; }        //must go in start rather than awake or big fucky wucky happens :3
        PlayerController = pc;
        canDealDmg = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canDealDmg)
        {
            //int dmgGiven = 10;        //putting it down here bc god hates us and tweaks = bad *apparently*
            //Debug.Log("Dmg given: " + dmgGiven);
            PlayerController.dmgToPC = dmgGiven;
            PlayerController.TakeDmg();
            //StartCoroutine(DMGCooldown());
        }
    }
}

