using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

/*
    Some edits by Leonardo Caballero
*/
public class CarHealthBehavior : MonoBehaviour
{
    // StackTrace stackTrace;
    public GameObject respawnPlatform;
    public GameObject cheatRespawnPlatform;
    public GameObject modelHolder;
    public GameObject carObject;
    public GameObject explosionEffect;
    public GameObject teleportEffect;
    public GameObject deathFade;

    public float invulnerabilityTime = 3f;
    private float _invulnerabilityTimer = 0f;
    private bool _canTakeDamage = true;

    public float shieldsMax = 100f;
    public float currentExtraShields = 0;
    public float currentPersonalShields = 0;
    public float healthCurrent = 100f;
    public float healthMax = 100f;
    private float _tempDamage;
    private float _shieldTotal;

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
    public Image damageVignette;
    public Color vignetteColor;
    public Sprite engineGreen;
    public Sprite engineYellow;
    public Sprite engineOrange;
    public Sprite engineRed;
    public float engineFlashFreq = .4f;
    private bool engineIsFlash = false;
    private bool isFlashing = false;



    Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    float extra = 0;

    Rigidbody _carBodyHolder;
    public RaycastCar raycastCarHolder;


    private void Start()
    {
        _vehicleInput = GetComponent<VehicleInput>();
        //InvokeRepeating("healthCooldown", 0, healFreq);
        healthCurrent = healthMax;

        gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[1];
        colorKey[0].color = vignetteColor;
        colorKey[0].time = 0.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);

        _carBodyHolder = carObject.GetComponent<Rigidbody>();
        raycastCarHolder = GetComponent<RaycastCar>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            if (healthCurrent > healthMax)
            {
                healthCurrent = healthMax;
            }

            if (healthCurrent == healthMax && gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }
            #region HealthFillBar
            if (healthFillBar != null)
            {
                healthFillBar.fillAmount = healthCurrent / healthMax;

                if (healthFillBar.fillAmount <= 0.20)
                {
                    if (!isFlashing)
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
            if (extraHealthFillBar != null)
            {
                extraHealthFillBar.fillAmount = _shieldTotal / shieldsMax;
            }
            #endregion

            #region Vignette
            if (_vehicleInput)
            {
                extra += Time.deltaTime;

                if (extra > 0)
                {

                    extra = 0;
                }




                if (damageVignette.color.a > 0)
                {
                    //Debug.Log(damageVignette.color.a);
                    damageVignette.color -= new Color(0, 0, 0, .002f);
                }//damageVignette.color = gradient.Evaluate((healthCurrent + extra) / healthMax);
                 //float scale = Mathf.Lerp(1, 2.5f, (healthCurrent + extra) / healthMax);
                 //  damageVignette.gameObject.transform.localScale = new Vector3(scale, scale, scale);

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

    public void Kill()
    {

        // stackTrace = new StackTrace();
        //print("KILL !! " + stackTrace.GetFrame(1).GetMethod().Name);


        AudioManager.instance.Play("Death", transform);
        isDead = true;
        Instantiate(explosionEffect, gameObject.transform.position, gameObject.transform.rotation);
        if (_vehicleInput)
        {
            deathFade.GetComponent<Animator>().Play("DeathFadeIn");
        }
        raycastCarHolder.enabled = false;
        GetComponent<VehicleAbilityBehavior>().enabled = false;
        GameObject respawnInstance = Instantiate(respawnPlatform);
        respawnInstance.GetComponent<RespawnPlatformBehavior>().SetPlayer(this.gameObject, modelHolder);
        _carBodyHolder.useGravity = false;
        _carBodyHolder.isKinematic = true;

        if (_vehicleInput)
        {
            gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Clear();
        }
    }

    public void AICheatKill()
    {
        AudioManager.instance.Play("Death", transform);
        isDead = true;
        Instantiate(explosionEffect, gameObject.transform.position, gameObject.transform.rotation);
        if (_vehicleInput)
        {
            deathFade.GetComponent<Animator>().Play("DeathFadeIn");
        }
        raycastCarHolder.enabled = false;
        GetComponent<VehicleAbilityBehavior>().enabled = false;
        GameObject respawnInstance = Instantiate(cheatRespawnPlatform);
        respawnInstance.GetComponent<RespawnPlatformBehavior>().SetPlayer(this.gameObject, modelHolder);
        _carBodyHolder.useGravity = false;
        _carBodyHolder.isKinematic = true;

        if (_vehicleInput)
        {
            gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Clear();
        }
    }

    public void Respawn()
    {
        if (_vehicleInput)
        {
            damageVignette.color -= new Color(0, 0, 0, damageVignette.color.a);
        }
        isFlashing = false;
        isDead = false;
        _canTakeDamage = false;
        StartCoroutine(Invulnerability());
        healthCurrent = healthMax;
        CancelInvoke("FlashRedEngine");
        if (_vehicleInput)
        {

            engineImage.color = Color.white;
            engineIsFlash = false;
            deathFade.GetComponent<Animator>().Play("DeathFadeOut");
        }
        raycastCarHolder.enabled = true;
        GetComponent<VehicleAbilityBehavior>().enabled = true;
        _carBodyHolder.useGravity = true;
        _carBodyHolder.isKinematic = false;
        HeatAbility bAbility = GetComponent<HeatAbility>();
        AudioManager.instance.Play("Respawn complete", transform);
        if (bAbility != null)
        {
            bAbility.DeactivateAbility();
        }
        if (_vehicleInput)
        {
            gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Clear();
        }
        //GetComponent<RaycastCar>().setDrifting(false);
        // GetComponent<RaycastCar>().SetIsBoosting(false);

    }

    private IEnumerator Invulnerability()
    {
        _invulnerabilityTimer = invulnerabilityTime;
        while (!_canTakeDamage)
        {
            _invulnerabilityTimer -= Time.deltaTime;
            if (_invulnerabilityTimer <= 0)
            {
                _canTakeDamage = true;
            }
            yield return null;
        }
    }

    public void Teleport()
    {
        if (_canTeleport)
        {
            _canTeleport = false;
            isDead = true;
            teleportCDImage.fillAmount = 1;
            teleDark.SetActive(true);
            AudioManager.instance.Play("Teleport", transform);
            StartCoroutine(teleportCooldownTimer());
            Instantiate(teleportEffect, gameObject.transform.position, gameObject.transform.rotation);
            deathFade.GetComponent<Animator>().Play("DeathFadeIn");
            raycastCarHolder.enabled = false;
            GetComponent<VehicleAbilityBehavior>().enabled = false;
            GameObject respawnInstance = Instantiate(respawnPlatform);
            respawnInstance.GetComponent<RespawnPlatformBehavior>().SetPlayer(this.gameObject, modelHolder);
            _carBodyHolder.useGravity = false;
            _carBodyHolder.isKinematic = true;
        }
    }

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

    private void healthCooldown()
    {
        healthCurrent += healAmount;
    }

    public void DamageCar(float damage, int killerID)
    {
        if (_canTakeDamage)
        {

            if (_shieldTotal > 0)
            { //if you have shields

                _tempDamage = damage;

                if (currentExtraShields > 0)
                {//if you have extra shields

                    _tempDamage = damage - currentExtraShields; // amount of damage left over after abosrbed by shields

                    currentExtraShields -= damage;

                    if (currentExtraShields < 0)
                    {
                        currentExtraShields = 0;
                    }
                    if (_tempDamage < 0)
                    {//makes sure we don't heal player later on.
                        _tempDamage = 0;
                    }
                }

                if (currentPersonalShields > 0 && _tempDamage > 0)
                { // if you have personal shields and the damage wasn't absorbed already.
                    if (_tempDamage != damage)
                    {//if some damage has already been absorbed, we assume tempDamage is valid

                        float _tD = _tempDamage;
                        _tempDamage -= currentPersonalShields;

                        currentPersonalShields -= _tD;
                    }
                    else
                    {
                        _tempDamage = damage - currentPersonalShields; // amount of damage left over after absorbed by shields

                        currentPersonalShields -= damage;

                    }

                    if (_tempDamage < 0)
                    {//makes sure _tempDamage doesn't heal the player later on
                        _tempDamage = 0;
                    }
                    if (currentPersonalShields < 0)
                    {
                        currentPersonalShields = 0;
                    }
                }
                _shieldTotal = currentPersonalShields + currentExtraShields;
                if (_tempDamage > 0)
                { //If any damage got through
                    healthCurrent -= _tempDamage;
                    if (_vehicleInput)
                    {
                        damageVignette.color = new Color(damageVignette.color.r, damageVignette.color.g, damageVignette.color.b, 1 - ((healthCurrent + extra) / healthMax));
                    }
                }
            }

            else
            { //if no shields
                healthCurrent -= damage;
                extra = -20;
                if (healthCurrent > 0)
                {
                    if (_vehicleInput)
                    {
                        damageVignette.color = new Color(damageVignette.color.r, damageVignette.color.g, damageVignette.color.b, 1 - ((healthCurrent + extra) / healthMax));
                    }
                }
            }

            if (healthCurrent <= 0)
            { //kill player
                //Debug.Log("Player: " + gameObject.transform.parent.name + " should be killed by car # : " + killerID);
                if (killerID <= DataManager.instance.playerInfo.Length && killerID != raycastCarHolder.playerID && !isDead)
                { //if someone killed you and you didn't cause your death.
                  //    Debug.Log("Kill was properly awareded!");
                    DataManager.instance.playerInfo[killerID - 1].numKills++;
                }

                Kill();
            }
        }
    }

    public void DamageCarTrue(float damage)
    {
        healthCurrent -= damage;
        if (healthCurrent <= 0)
        {
            Kill();
        }
    }

    public float GetPersonalShieldAmount()
    {
        return currentPersonalShields;
    }

    public void SetPersonalShieldAmount(float extraHP)
    {
        currentPersonalShields = extraHP;
        _shieldTotal = currentPersonalShields + currentExtraShields;
    }

    public void AddPersonalShields(float personalShields)
    {
        if ((currentExtraShields + currentPersonalShields + personalShields) <= shieldsMax)
        {//if you wouldn't gain too much shield
            currentPersonalShields += personalShields;
        }
        else
        {//otherwise, just gain up to the max
            currentPersonalShields += (shieldsMax - currentPersonalShields - currentExtraShields);
        }
        _shieldTotal = currentPersonalShields + currentExtraShields;
    }

    public void AddExtraShields(float extraShields)
    {
        if ((currentExtraShields + currentPersonalShields + extraShields) <= shieldsMax)
        {//if you wouldn't gain too much shield
            currentExtraShields += extraShields;
        }
        else
        {//otherwise, just gain up to the max
            currentExtraShields += (shieldsMax - currentPersonalShields - currentExtraShields);
        }
        _shieldTotal = currentPersonalShields + currentExtraShields;
    }

    public void RemoveExtraShields(float extraShields)
    {
        if(currentExtraShields > 0)
        {
            currentExtraShields -= extraShields;
            if(currentExtraShields < 0)
            {
                currentExtraShields = 0;
            }

            _shieldTotal = currentPersonalShields + currentExtraShields; // this line is in the IF so that we avoid unnecessary UI redraws

        }
    }

    public float GetExtaHealthMax()
    {
        return shieldsMax;
    }

    public void SetExtraHealthMax(float extraHealthMax)
    {
        shieldsMax = extraHealthMax;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("KillBox"))
        {
            Kill();
        }
    }


}

