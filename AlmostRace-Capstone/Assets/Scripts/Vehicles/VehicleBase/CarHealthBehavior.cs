using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

/*
 * Written by Eddie Borissov initially, edited by basically everyone else on the team at one point.
*/
public class CarHealthBehavior : Health
{
   // StackTrace stackTrace;
    public GameObject respawnPlatform;
    public GameObject modelHolder;
    public GameObject carObject;
    public GameObject explosionEffect;
    public GameObject teleportEffect;
    public GameObject deathFade;

    private float _extraHPMax = 120f;
    private float _extraHP = 0;
     
    public float healAmount = 1f;
    public float respawnSecs = 3f;
    public float teleportCooldown = 5f;
    public float healFreq = 2f;
    public bool isDead;
    private VehicleInput _vehicleInput;
    private bool _canTeleport = true;

    [Header("UI Variables")]
    [Space(30)]
    public Image teleportCDImage;
    public GameObject teleDark;
    public Image extraHealthFillBar;
    public Image healthFillBar;
    public Image engineImage;
    public Sprite engineGreen;
    public Sprite engineYellow;
    public Sprite engineOrange;
    public Sprite engineRed;
    public float engineFlashFreq = .4f;
    private bool engineIsFlash = false;
    private bool isFlashing = false;
    private void Start()
    {
        _vehicleInput = GetComponent<VehicleInput>();
        InvokeRepeating("healthCooldown", 0, healFreq);
        healthCurrent = healthMax;
    }
    // Update is called once per frame
    void Update()
    {
        if (_vehicleInput != null)
        {
            if (Input.GetAxis(_vehicleInput.respawn) > 0 && _vehicleInput.getStatus() && !GetComponent<HotSpotVehicleAdministration>().holdingTheBot)
            {
                Teleport();
            }
        }

        if (!isDead)
        {
            if (healthCurrent > healthMax)
            {
                healthCurrent = healthMax;
            }

            if (healthCurrent <= 0)
            {
                Kill();
            }

            if (healthCurrent == healthMax && gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }
            #region HealthFillBar
            if (healthFillBar != null)
            {
                healthFillBar.fillAmount = healthCurrent/healthMax ;

                if (healthFillBar.fillAmount <= 0.20)
                {
                    if(!isFlashing)
                    {
                        isFlashing = true;
                        InvokeRepeating("FlashRedEngine", 0, engineFlashFreq);
                    }
                  
                }
                else if (healthFillBar.fillAmount < 0.33)
                {
                    engineImage.sprite = engineRed;
                    healthFillBar.color = new Color32(232, 27, 66, 255);
                }

                else if (healthFillBar.fillAmount < 0.50)
                {
                    engineImage.sprite = engineOrange;
                    healthFillBar.color = new Color32(217, 121, 39, 255);
                }

                else if (healthFillBar.fillAmount < 0.75)
                {
                    engineImage.sprite = engineYellow;
                    healthFillBar.color = new Color32(220, 189, 38, 255);

                }
                else
                {
                    engineImage.sprite = engineGreen;
                    healthFillBar.color = new Color32(53, 180, 74, 255);
                    /*heat25.enabled = false;
                    heat50.enabled = false;
                    heat75.enabled = false;
                    heat90.enabled = false;*/
                }

            }
            #endregion

            #region ExtraHealthFillBar
            if(extraHealthFillBar != null)
            {
                extraHealthFillBar.fillAmount = _extraHP / _extraHPMax;
            }
            #endregion
        }

    }

    private void FlashRedEngine()
    {
        engineImage.sprite = engineRed;
        healthFillBar.color = new Color32(232, 27, 66, 255);
        if (!engineIsFlash)
        {//if hasn't flashed and is still red
            engineImage.color = Color.black;
            engineIsFlash = true;
        }
        else
        {
            engineImage.color = Color.white;
            engineIsFlash = false;
        }

        if (healthFillBar.fillAmount > 0.20)
        {
            CancelInvoke("FlashRedEngine");
            engineImage.color = Color.white;
            engineIsFlash = false;
            isFlashing = false;
        }
    }

    public override void Kill()
    {
        AudioManager.instance.Play("Death");
        isDead = true;
        Instantiate(explosionEffect, gameObject.transform.position, gameObject.transform.rotation);
        deathFade.GetComponent<Animator>().Play("DeathFadeIn");
        GetComponent<RaycastCar>().enabled = false;
        GetComponent<VehicleAbilityBehavior>().enabled = false;
        GameObject respawnInstance = Instantiate(respawnPlatform);
        respawnInstance.GetComponent<RespawnPlatformBehavior>().SetPlayer(this.gameObject, modelHolder);
        carObject.GetComponent<Rigidbody>().useGravity = false;
        carObject.GetComponent<Rigidbody>().isKinematic = true;

        gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Clear();
    }

    public void Respawn()
    {
        isFlashing = false;
        CancelInvoke("FlashRedEngine");
        engineImage.color = Color.white;
        engineIsFlash = false;
        healthCurrent = healthMax;
        isDead = false;
        deathFade.GetComponent<Animator>().Play("DeathFadeOut");
        GetComponent<RaycastCar>().enabled = true;
        GetComponent<VehicleAbilityBehavior>().enabled = true;
        carObject.GetComponent<Rigidbody>().useGravity = true;
        carObject.GetComponent<Rigidbody>().isKinematic = false;
        HeatAbility bAbility = GetComponent<HeatAbility>();
        AudioManager.instance.Play("Respawn complete");
        if (bAbility != null)
        {
            bAbility.DeactivateAbility();
        }
        gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Clear();
        //GetComponent<RaycastCar>().setDrifting(false);
       // GetComponent<RaycastCar>().SetIsBoosting(false);

    }


    /// <summary>
    /// Teleports the player near the hotspot bot
    /// </summary>
    public void Teleport()
    {
        if (_canTeleport)
        {
            _canTeleport = false;
            isDead = true;
            teleportCDImage.fillAmount = 1;
            teleDark.SetActive(true);
            AudioManager.instance.Play("Teleport");
            StartCoroutine(teleportCooldownTimer());
            Instantiate(teleportEffect, gameObject.transform.position, gameObject.transform.rotation);
            deathFade.GetComponent<Animator>().Play("DeathFadeIn");
            GetComponent<RaycastCar>().enabled = false;
            GetComponent<VehicleAbilityBehavior>().enabled = false;
            GameObject respawnInstance = Instantiate(respawnPlatform);
            respawnInstance.GetComponent<RespawnPlatformBehavior>().SetPlayer(this.gameObject, modelHolder);
            carObject.GetComponent<Rigidbody>().useGravity = false;
            carObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    /// <summary>
    /// Prevents players from spamming the teleport functionality
    /// </summary>
    /// <returns></returns>
    private IEnumerator teleportCooldownTimer()
    {
        float tempTime = teleportCooldown;
        while (tempTime > 0)
        {
            tempTime -= Time.deltaTime;
            teleportCDImage.fillAmount = tempTime / teleportCooldown;
            yield return null;
        }
        teleDark.SetActive(false);
        _canTeleport = true;
    }

    /// <summary>
    /// Provides a health regen effect
    /// </summary>
    private void healthCooldown()
    {
        healthCurrent += healAmount;
    }

    /// <summary>
    /// Depreciated damage function, delete where found and switch to DamageHealth(); Once all instances are found
    /// and removed, delete this function.
    /// </summary>
    /// <param name="damage"></param>
    /*public void DamageCar(float damage)
    {
        Debug.Log("USING OLD DamageCar() FUNCTION, SWITCH IT TO DamageHealth()!!");
    }*/

    /// <summary>
    /// Depreciated damage function, delete where found and switch to DamageHealth(); Once all instances are found
    /// and removed, delete this function.
    /// </summary>
    /// <param name="damage"></param>
    /*public void DamageCarTrue(float damage)
    {
        Debug.Log("USING OLD DamageCarTrue() FUNCTION, SWITCH IT TO DamageHealthTrue()!!");

    }*/

    public float GetExtraHealth()
    {
        return _extraHP;
    }

    public void SetExtraHealth(float extraHP)
    {
        _extraHP = extraHP;
    }

    public float GetExtaHealthMax()
    {
        return _extraHPMax;
    }

    public void SetExtraHealthMax(float extraHealthMax)
    {
        _extraHPMax = extraHealthMax;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("KillBox"))
        {
            Kill();
        }
    }

    /// Inherited Functions!

    public override void DamageHealth(float damageAmount)
    {
        if (_extraHP > 0)
        { //if you have _extraHP

            //damage left over after it's delt to the _extraHP
            float _tempDamage = damageAmount - _extraHP;

            if (_tempDamage < 0)
            {//makes sure _tempDamage doesn't heal the player later on
                _tempDamage = 0;
            }

            _extraHP -= damageAmount; //deals the damage to the _extraHP

            if (_extraHP <= 0)
            { //If you have no _extraHP left
                healthCurrent -= _tempDamage;
            }
        }
        else
        {
            healthCurrent -= damageAmount;
        }

    }

    public override void DamageHealthTrue(float trueDamageAmount)
    {
        healthCurrent -= trueDamageAmount;
    }

    public override float GetHealth()
    {
        throw new System.NotImplementedException();
    }

    public override void SetHealth(float newHealth)
    {
        throw new System.NotImplementedException();
    }

    public override void AddHealth(float healthToAdd)
    {
        throw new System.NotImplementedException();
    }
}

