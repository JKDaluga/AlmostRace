using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainTrain_Attack : Ability
{
    [Header("Lightning Ball Variables")]
    [Space(30)]

    public Transform muzzle;

    /// <summary>
    /// The Lightning Ball
    /// </summary>
    public GameObject lightningBall;

    /// <summary>
    /// How much damage the Ball does while it's attached
    /// </summary>
    public float lightningBallDamage;

    /// <summary>
    /// How long the Ball stays attached to an enemy and does damage
    /// </summary>
    public float lightningBallDuration;

    /// <summary>
    /// How often the Ball delivers its damage.
    /// </summary>
    public float lightningBallFrequency;

    /// <summary>
    /// How fast the Ball travels
    /// </summary>
    public float lightningBallSpeed;


    [Header("Skull Variables")]

    public Animator skull;

    public List<SpinObject> gearsToSpin = new List<SpinObject>();

    public override void ActivateAbility()
    {
        AudioManager.instance.Play("Pain Train Attack", transform);
        //Debug.Log("Ability should be activated!");
        GameObject spawnedLightningBall = Instantiate(lightningBall, muzzle.position, muzzle.rotation);

        spawnedLightningBall.GetComponent<PainTrain_LightningBall>().SetProjectileInfo(0, lightningBallSpeed, 0);
        spawnedLightningBall.GetComponent<PainTrain_LightningBall>().SetImmunePlayer(gameObject);

        spawnedLightningBall.GetComponent<PainTrain_LightningBall>().GiveInfo(lightningBallDamage, lightningBallDuration, lightningBallFrequency);
        Destroy(spawnedLightningBall, 10);

     
       
       
    }

    public override void DeactivateAbility()
    {

    }

    public override void AbilityInUse()
    {
        skull.SetTrigger("SkullUp");
        //skull.StartCoroutine(skull.RaiseSkull());
       //skull.StartCoroutine(skull.FaceSkullUp());
    }

    public override void AbilityOffOfCooldown()
    {
        skull.SetTrigger("SkullDown");
        //skull.StartCoroutine(skull.LowerSkull());
        //skull.StartCoroutine(skull.FaceSkullDown());
    }

    public override void AbilityOnCooldown()
    {
        skull.SetTrigger("SkullDown");
        //skull.StartCoroutine(skull.RaiseSkull());
        //skull.StartCoroutine(skull.FaceSkullUp());
    }

}
