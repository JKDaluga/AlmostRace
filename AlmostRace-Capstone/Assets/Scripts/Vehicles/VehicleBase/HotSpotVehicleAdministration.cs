using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpotVehicleAdministration : MonoBehaviour
{
    public bool holdingTheBot;
    private GameObject HotSpotBotHeld;
    public float initialHypeGain, gradualHypeGain, hypeWaitTime;
    private float hypeTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (holdingTheBot)
        {
            hypeTimer += Time.deltaTime;
            if(hypeTimer >= hypeWaitTime)
            {
                HypeGain(gradualHypeGain);
                hypeTimer = 0;
            }
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
                GetComponent<CarHeatManager>().heatCurrent = 0;
                HypeGain(initialHypeGain);
                hypeTimer = 0;
            }
        }
    }

    void HypeGain(float added)
    {
        GetComponent<VehicleHypeBehavior>().AddHype(added);
    }
}
