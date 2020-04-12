using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineSwapTrigger : MonoBehaviour
{
    public GameObject[] orderedSplines;
    public List<RaycastCar> aiCars;

    RaceManager rM;

    public bool arenaEntrance;

    private void Awake()
    {
        aiCars = new List<RaycastCar>();

        rM = FindObjectOfType<RaceManager>();

        orderedSplines = rM.orderedSplines;
    }

    private void Update()
    {

        if(aiCars.Count == 0)
        {
            SplineSwapTrigger temp = FindObjectOfType<SplineSwapTrigger>();
            aiCars = temp.aiCars;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RaycastCar>())
        {
            rM.AISplineIndex++;
            updateAI();

            GetComponent<Collider>().enabled = false;
        }
    }

    public void updateAI()
    {
        foreach (RaycastCar i in aiCars)
        {
            i.GetComponent<AIBehaviour>().SwapSpline(orderedSplines[rM.AISplineIndex].GetComponent<SplinePlus>(), arenaEntrance);
        }
    }
}
