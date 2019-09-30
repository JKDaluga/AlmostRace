using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHypeBehavior : MonoBehaviour
{
    [Tooltip("They are either player 1, 2, 3, or 4? etc.")] public int playerNumber;
    private HypeSystem _hypeSystem;
    private float _hypeAmount;

    void Start()
    {
        if(GameObject.FindGameObjectWithTag("GameManager") != null)
        {
            _hypeSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HypeSystem>();
            _hypeSystem.VehicleAssign(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (playerNumber == 1)
            {
                AddHype(5);
            }
            else if (playerNumber == 2)
            {
                AddHype(4);
            }
            else if (playerNumber == 3)
            {
                AddHype(3);
            }
            else if (playerNumber == 4)
            {
                AddHype(2);
            }
        }
    }

    private void AddHype(float hypeToAdd)
    {
        _hypeAmount += hypeToAdd;
        _hypeSystem.VehicleSort();
    }

    private void SubtractHype(float hypeToSubtract)
    {
        _hypeAmount -= hypeToSubtract;
        _hypeSystem.VehicleSort();
    }

    public float GiveHypeAmount()
    {
        return _hypeAmount;
    }

}
