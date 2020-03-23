using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VehicleAbilityDescriptions : MonoBehaviour
{
    public string vehicleName;
    public TextMeshProUGUI[] vehicleAbilityInfoText;

    public TextMeshProUGUI GetSelectecAbilityText(int givenSelectedAbility)
    {
        return vehicleAbilityInfoText[givenSelectedAbility];
    }

    public string GetVehicleName()
    {
        return vehicleName;
    }
}
