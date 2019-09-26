using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HypeSystem : MonoBehaviour
{
    public GameObject[] playerVehicle = new GameObject[4];
    public Dictionary<GameObject, float> dictionary = new Dictionary<GameObject, float>(4);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AssignVehicleSlot(GameObject player, int playerNumber)
    {
        for (int i = 0; i < playerVehicle.Length; i++)
        {
            if (i == playerNumber - 1)
            {
                playerVehicle[i] = player;
                break;
            }
        }
    }

    public void VehicleAssign(GameObject player)
    {
        dictionary.Add(player, 0);
        VehicleSort();
    }

    public void AssignHype(GameObject player, float hypeAmount)
    {
        dictionary[player] += hypeAmount;
        VehicleSort();
    }

    public void SubtractHype(GameObject player, float hypeAmount)
    {
        dictionary[player] -= hypeAmount;
        VehicleSort();
    }

    private void VehicleSort()
    {
        // Order by values
        var items = from pair in dictionary
                    orderby pair.Value ascending
                    select pair;
    }
}
