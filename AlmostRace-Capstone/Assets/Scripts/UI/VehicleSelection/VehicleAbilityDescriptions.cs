using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleAbilityDescriptions : MonoBehaviour
{
    public string vehicleName;
    public Image[] vehicleAbilityInfoText;

    public Image GetSelectecAbilityText(int givenSelectedAbility)
    {
        return vehicleAbilityInfoText[givenSelectedAbility];
    }

    public string GetVehicleName()
    {
        return vehicleName;
    }
}
