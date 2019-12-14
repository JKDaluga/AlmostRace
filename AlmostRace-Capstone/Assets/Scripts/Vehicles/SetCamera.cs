using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string layer = "Player" + GetComponent<VehicleInput>().getPlayerNum();
        transform.parent.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = LayerMask.NameToLayer(layer);
        transform.parent.GetComponentInChildren<Camera>().cullingMask += (1 << LayerMask.NameToLayer(layer));

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

        print(transform.parent.GetComponentInChildren<CinemachineImpulseListener>().m_ChannelMask.ToString() + "-" + GetComponent<VehicleInput>().playerNumber);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
