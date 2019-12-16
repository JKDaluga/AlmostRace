using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 * Robyn Riley. Script for assigning correct cameras
 * Script interacts with the cinemachine system to assign correct camera layers
 * and align impulse sources and listeners for camera shake
 */

public class SetCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Assigns the cinemachine camera layer and camera's visible layers to match
        string layer = "Player" + GetComponent<VehicleInput>().getPlayerNum();
        transform.parent.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = LayerMask.NameToLayer(layer);
        transform.parent.GetComponentInChildren<Camera>().cullingMask += (1 << LayerMask.NameToLayer(layer));


        //Gets the player's number and assigns them a correct impulse channel based on that
        switch (GetComponent<VehicleInput>().playerNumber)
        {
            case 1:
                transform.parent.GetComponentInChildren<CinemachineImpulseListener>().m_ChannelMask = 1;
                GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_ImpulseChannel = 1;
                break;
            case 2:
                transform.parent.GetComponentInChildren<CinemachineImpulseListener>().m_ChannelMask = 2;
                GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_ImpulseChannel = 2;
                break;
            case 3:
                transform.parent.GetComponentInChildren<CinemachineImpulseListener>().m_ChannelMask = 4;
                GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_ImpulseChannel = 4;
                break;
            case 4:
                transform.parent.GetComponentInChildren<CinemachineImpulseListener>().m_ChannelMask = 8;
                GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_ImpulseChannel = 8;
                break;
        }


        
    }
}
