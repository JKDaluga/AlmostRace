using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_ShieldHelper : MonoBehaviour
{
    // Pas on function refs to the void surge animation wrapper so that we can call these functions. 
    public GameObject parentRef;

    void StopAnim()
    {
        parentRef.GetComponent<VoidWasp_ShieldBehaviour>().DestroyInteractable();
    }
}
