using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObjectsOnStart : MonoBehaviour
{
    public GameObject[] objectsToDeactivate;

    void Start()
    {
        for (int i = 0; i < objectsToDeactivate.Length; i++)
        {
            objectsToDeactivate[i].SetActive(false);
        }
    }
}
