using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    private PlayerController PlayerController;
    
    bool canDealDmg;
    public float dmgCooldown = 1.5f;

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
            int dmgGiven = 10;        //putting it down here bc god hates me
            Debug.Log("Dmg given: " + dmgGiven);
            Debug.Log(":3 uwu nuzzles (touches)");
            PlayerController.dmgToPC = dmgGiven;
            PlayerController.TakeDmg();
            StartCoroutine(DMGCooldown());
        }
    }

    IEnumerator DMGCooldown()
    {
        canDealDmg = false;
        Debug.Log("Can dmg: " + canDealDmg);
        yield return new WaitForSeconds(1.5f);          //yea again for some reason i'd MUCH rather not hard code this value but ig its either this or instantaneous death
        canDealDmg = true;
        Debug.Log("Can dmg: " + canDealDmg);
    }
}

