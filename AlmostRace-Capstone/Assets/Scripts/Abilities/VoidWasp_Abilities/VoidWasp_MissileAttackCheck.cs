using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_MissileAttackCheck : MonoBehaviour
{
    public SolarCycle_HomingMissile homingMissileScript;

    private void OnTriggerEnter(Collider collision)
    {
        homingMissileScript.AttackTriggered(collision.gameObject);
    }
}
