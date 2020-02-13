using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jake Velicer
    Purpose: Contains the behavior for vehciles picking up the hot spot bot,
    and the behavior for dropping it.
    */

public class HotSpotVehicleAdministration : MonoBehaviour
{
    public bool holdingTheBot;
    private GameObject HotSpotBotHeld;
    public float initialHypeGain, gradualHypeGain, hypeWaitTime;
    public GameObject[] botCapturedAesthetic;
    private float hypeTimer;

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
        foreach (GameObject item in botCapturedAesthetic)
        {
            item.SetActive(true);
        }
    }

    public void DropTheBot()
    {
        holdingTheBot = false;
        HotSpotBotHeld.SetActive(true);
        HotSpotBotHeld.GetComponent<HotSpotBotBehavior>().SetBeingHeld(false);
        StartCoroutine(HotSpotBotHeld.GetComponent<HotSpotBotBehavior>().SetPosition(transform.position));
        HotSpotBotHeld = null;
        AudioManager.instance.Play("HotSpot Lost");
        foreach (GameObject item in botCapturedAesthetic)
        {
            item.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("HotSpot"))
        {
            if(!other.gameObject.transform.parent.GetComponent<HotSpotBotBehavior>().GetBeingHeld()
            && !holdingTheBot && HotSpotBotHeld == null)
            {
                HoldTheBot(other.gameObject);
                GetComponent<CarHealthBehavior>().healthCurrent = GetComponent<CarHealthBehavior>().healthMax;
                HypeGain(initialHypeGain);
                hypeTimer = 0;
                AudioManager.instance.Play("HotSpot Attachment");
            }
        }
    }

    void HypeGain(float added)
    {
        GetComponent<VehicleHypeBehavior>().AddHype(added, "Bot Grabbed!");
    }
}
