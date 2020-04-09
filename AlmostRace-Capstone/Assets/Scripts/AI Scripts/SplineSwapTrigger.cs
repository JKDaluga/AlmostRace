using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineSwapTrigger : MonoBehaviour
{
    public GameObject[] orderedSplines;
    public List<RaycastCar> aiCars;
    public int currSpline;

    private void Awake()
    {
        aiCars = new List<RaycastCar>();
        currSpline = 0;
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RaycastCar>())
        {
            currSpline++;
            updateAI();

            GetComponent<Collider>().enabled = false;
        }
    }

    public void updateAI()
    {
        foreach (RaycastCar i in aiCars)
        {
            i.GetComponent<AIBehaviour>().SwapSpline(orderedSplines[currSpline].GetComponent<SplinePlus>());
        }
    }
}
