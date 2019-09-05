using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierEvent : MonoBehaviour
{
    public GameObject[] objectsToDisable;
    public GameObject[] particlesToEnable;

    public void BarrierEventActiavtion()
    {
        for (int i = 0; i < objectsToDisable.Length; i++)
        {
            objectsToDisable[i].SetActive(false);
        }
        for (int i = 0; i < particlesToEnable.Length; i++)
        {
            if (particlesToEnable[i] != null)
            {
                particlesToEnable[i].SetActive(true);
            }
        }

    } 
}
