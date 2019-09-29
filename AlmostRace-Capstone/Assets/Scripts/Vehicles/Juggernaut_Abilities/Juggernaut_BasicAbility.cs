using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juggernaut_BasicAbility : BasicAbility
{

    public Juggernaut_Drill drillScript;

    public float drillDamage;
    public float drillFrequency;






    public override void ActivateAbility()
    {
        if (!drillScript.GettIsSpinning())
        {
            drillScript.SetIsSpinning(true);
        }

            
    }

    public override void DeactivateAbility()
    {
        if (drillScript.GettIsSpinning())
        {
            drillScript.SetIsSpinning(false);
        }
    }

    protected override void AddHeat()
    {
        throw new System.NotImplementedException();
    }

}
