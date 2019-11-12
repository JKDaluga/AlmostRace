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
        GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = LayerMask.NameToLayer(layer);
        GetComponentInChildren<Camera>().cullingMask += (1 << LayerMask.NameToLayer(layer));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
