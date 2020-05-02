using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainTrain_Shield : CooldownAbility
{

    public float shieldHealth;
    public GameObject shockShield;
    private PainTrain_ShieldShock _shockShieldScript;
    public List<GameObject> shields;
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
        foreach (GameObject shield in shields)
        {
            shield.GetComponent<ParticleSystem>().Stop();
        }
    }

    public override void ActivateAbility()
    {
        AudioManager.instance.Play("Pain Train Shield", transform);
        shockShield.SetActive(true);
        if (_carHealthScript != null)
        {
            _carHealthScript.AddPersonalShields(shieldHealth);
            _shockShieldScript.TurnOnShieldShock();
        }
        foreach (GameObject shield in shields)
        {
            shield.GetComponent<ParticleSystem>().Play();
        }

        //ribTipsSmall.SetActive(false);
        // ribTipsMedium.SetActive(true);
        // ribTipsLarge.SetActive(false);
    }

    public override void DeactivateAbility()
    {
        if (_carHealthScript != null)
        {
            _carHealthScript.SetPersonalShieldAmount(0);
            _shockShieldScript.TurnOffShieldShock();
        }
        shockShield.SetActive(false);
        foreach (GameObject shield in shields)
        {
            shield.GetComponent<ParticleSystem>().Stop();
            shield.GetComponent<ParticleSystem>().Clear();
        }
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
