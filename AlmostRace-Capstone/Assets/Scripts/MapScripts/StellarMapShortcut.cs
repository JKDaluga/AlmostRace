using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StellarMapShortcut : Shortcut
{
    public GameObject[] elementsToTurnOFF;

    public override void ActivateAbility()
    {
        foreach (GameObject blocker in elementsToTurnOFF)
        {
            blocker.SetActive(false);
        }
    }

}
