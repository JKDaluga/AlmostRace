using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineSwapTrigger : MonoBehaviour
{
    public GameObject[] orderedSplines;
    public List<RaycastCar> aiCars;

    RaceManager rM;

    public bool arenaEntrance;

    public List<GameObject> passed;

    private void Awake()
    {
        aiCars = new List<RaycastCar>();

        passed = new List<GameObject>();

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
            if (!passed.Contains(other.gameObject))
            {
                other.GetComponent<RaycastCar>().activeSpline++;
                if (!other.GetComponent<VehicleInput>())
                {
                    updateAI(other.GetComponent<AIBehaviour>());
                }
                passed.Add(other.gameObject);
            }
        }
    }

    public void updateAI(AIBehaviour i)
    {
        i.SwapSpline(orderedSplines[i.GetComponent<RaycastCar>().activeSpline].GetComponent<SplinePlus>(), arenaEntrance);
    }
    
}
