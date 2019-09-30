using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class HypeSystem : MonoBehaviour
{
    //public Dictionary<GameObject, float> dictionary = new Dictionary<GameObject, float>();
    private List<GameObject> _vehicleList = new List<GameObject>();
    private Text[] _hypeAmountDisplay;

    // Start is called before the first frame update
    void Start()
    {
        _hypeAmountDisplay = new Text[_vehicleList.Count];

        for(int i = 0; i < _hypeAmountDisplay.Length; i++)
        {
            _hypeAmountDisplay[i] = GameObject.Find("HypeDisplay" + (i + 1)).GetComponent<Text>();
        }
        UIupdate();
    }

    public void VehicleAssign(GameObject player)
    {
        _vehicleList.Add(player);
    }

    public void VehicleSort()
    {
        // Order by values
        _vehicleList.Sort(
            delegate(GameObject p1, GameObject p2)
            {
                return p1.GetComponent<VehicleHypeBehavior>().GiveHypeAmount()
                .CompareTo(p2.GetComponent<VehicleHypeBehavior>().GiveHypeAmount());
            }
        );
        _vehicleList.Reverse();
        UIupdate();
    }

    private void UIupdate()
    {
        int i = 0;
        foreach(GameObject entry in _vehicleList)
        {
            _hypeAmountDisplay[i].text = entry.name.ToString() + ": " +
            entry.GetComponent<VehicleHypeBehavior>().GiveHypeAmount().ToString();
            i++;
        }
    }
}
