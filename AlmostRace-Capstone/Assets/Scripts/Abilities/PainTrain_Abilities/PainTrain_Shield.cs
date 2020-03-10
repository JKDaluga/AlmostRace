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
    public float smallSize = .8f;
    public float mediumSize = 2;
    public float bigSize = 5;

    public List<ParticleSystem> ribParticles; 

    // Start is called before the first frame update
    void Start()
    {
        _carHealthScript = gameObject.GetComponent<CarHealthBehavior>();
        _shockShieldScript = shockShield.GetComponent<PainTrain_ShieldShock>();
        _shockShieldScript.GiveInfo(zapDamage, zapCoolDown, gameObject);
        AbilityOffOfCooldown();
    }

    public override void ActivateAbility()
    {
       
  
        _carHealthScript.AddExtraHealth(shieldHealth);
        shockShield.SetActive(true);
        _shockShieldScript.TurnOnShieldShock();

        //ribTipsSmall.SetActive(false);
       // ribTipsMedium.SetActive(true);
       // ribTipsLarge.SetActive(false);


    }

    public override void DeactivateAbility()
    {
        _carHealthScript.AddExtraHealth(0);
        _shockShieldScript.TurnOffShieldShock();
        shockShield.SetActive(false);
    }

    public override void AbilityOnCooldown()
    {
        foreach (ParticleSystem ps in ribParticles)
        {
            var psMain = ps.main;
            psMain.startSize = smallSize;
        }
        //ribTipsSmall.SetActive(true);
       //ribTipsMedium.SetActive(false);
        //ribTipsLarge.SetActive(false);
    }

    public override void AbilityOffOfCooldown()
    {
        foreach (ParticleSystem ps in ribParticles)
        {
            var psMain = ps.main;
            psMain.startSize = mediumSize;
        }
        //ribTipsSmall.SetActive(false);
        //ribTipsMedium.SetActive(true);
        //ribTipsLarge.SetActive(false);
    }

    public override void AbilityInUse()
    {
        foreach (ParticleSystem ps in ribParticles)
        {
            var psMain = ps.main;
            psMain.startSize = bigSize;
        }

        //ribTipsSmall.SetActive(false);
       //ribTipsMedium.SetActive(false);
        //ribTipsLarge.SetActive(true);
    }
}
