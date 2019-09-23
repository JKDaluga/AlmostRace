using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypeSystem : MonoBehaviour
{
    public GameObject[] userVehicle = new GameObject[4];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignVehicleSlot(GameObject playerVehicle, int playerNumber)
    {
        for (int i = 0; i < userVehicle.Length; i++)
        {
            if (i == playerNumber - 1)
            {
                userVehicle[i] = playerVehicle;
                break;
            }
        }
    }
}
