using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainTrain_Shield : CooldownAbility
{

    public float shieldHealth;
    public GameObject shockShield;
    private PainTrain_ShieldShock _shockShieldScript;
    public float zapDamage;
    public float zapCoolDown = .75f;
    private CarHealthBehavior _carHealthScript;

    // Start is called before the first frame update
    void Start()
    {
        _carHealthScript = gameObject.GetComponent<CarHealthBehavior>();
        _shockShieldScript = shockShield.GetComponent<PainTrain_ShieldShock>();
        _shockShieldScript.GiveInfo(zapDamage, zapCoolDown, gameObject);
        shockShield.SetActive(false);
       
    }

    public override void ActivateAbility()
    {
        _carHealthScript.AddExtraHealth(shieldHealth);
        shockShield.SetActive(true);
        _shockShieldScript.TurnOnShieldShock();
    }

    public override void DeactivateAbility()
    {
        _carHealthScript.AddExtraHealth(0);
        _shockShieldScript.TurnOffShieldShock();
        shockShield.SetActive(false);
    }




}
