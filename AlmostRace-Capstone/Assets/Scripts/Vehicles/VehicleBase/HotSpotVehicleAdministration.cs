using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpotVehicleAdministration : MonoBehaviour
{
    public bool holdingTheBot;
    private GameObject HotSpotBotHeld;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // For testing
        if (Input.GetKeyDown(KeyCode.P))
        {
            DropTheBot();
        }
    }

    private void HoldTheBot(GameObject theBot)
    {
        holdingTheBot = true;
        HotSpotBotHeld = theBot.transform.parent.gameObject;
        HotSpotBotHeld.GetComponent<HotSpotBotBehavior>().SetBeingHeld(true);
        HotSpotBotHeld.SetActive(false);
    }

    public void DropTheBot()
    {
        holdingTheBot = false;
        HotSpotBotHeld.GetComponent<HotSpotBotBehavior>().SetBeingHeld(false);
        HotSpotBotHeld.GetComponent<HotSpotBotBehavior>().SetPosition(transform.position);
        HotSpotBotHeld.SetActive(true);
        HotSpotBotHeld = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("HotSpot"))
        {
            if(!other.gameObject.transform.parent.GetComponent<HotSpotBotBehavior>().GetBeingHeld()
            && !holdingTheBot && HotSpotBotHeld == null)
            {
                HoldTheBot(other.gameObject);
            }
        }
    }
}
