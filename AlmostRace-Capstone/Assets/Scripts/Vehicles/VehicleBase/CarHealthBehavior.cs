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
    public GameObject modelHolder;
    public GameObject carObject;
    public GameObject explosionEffect;
    public GameObject teleportEffect;
    public GameObject deathFade;
    public float _extraHPMax = 100f;
    public float _extraHP = 0;
    public float healthCurrent = 100f;
    public float healthMax = 100f;
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
    }
    // Update is called once per frame
    void Update()
    {
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

            #region Vignette
            if (GetComponent<VehicleInput>())
            {
                extra += Time.deltaTime;

                if (extra > 0)
                {
                    extra = 0;
                }

                damageVignette.color = gradient.Evaluate((healthCurrent + extra) / healthMax);
                float scale = Mathf.Lerp(1, 2.5f, (healthCurrent + extra) / healthMax);
                damageVignette.gameObject.transform.localScale = new Vector3(scale, scale, scale);
                
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

        //AudioManager.instance.Play("Death");
        AudioManager.instance.Play("Death", transform);
        isDead = true;
        Instantiate(explosionEffect, gameObject.transform.position, gameObject.transform.rotation);
        if (GetComponent<VehicleInput>())
        {
            deathFade.GetComponent<Animator>().Play("DeathFadeIn");
        }
        GetComponent<RaycastCar>().enabled = false;
        GetComponent<VehicleAbilityBehavior>().enabled = false;
        GameObject respawnInstance = Instantiate(respawnPlatform);
        respawnInstance.GetComponent<RespawnPlatformBehavior>().SetPlayer(this.gameObject, modelHolder);
        carObject.GetComponent<Rigidbody>().useGravity = false;
        carObject.GetComponent<Rigidbody>().isKinematic = true;

        if (gameObject.GetComponent<VehicleInput>())
        {
            gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Clear();
        }
    }

    public void Respawn()
    {
        isFlashing = false;
        isDead = false;
        healthCurrent = healthMax;
        CancelInvoke("FlashRedEngine");
        if (GetComponent<VehicleInput>())
        {

            engineImage.color = Color.white;
            engineIsFlash = false;              
            deathFade.GetComponent<Animator>().Play("DeathFadeOut");
        }
        GetComponent<RaycastCar>().enabled = true;
        GetComponent<VehicleAbilityBehavior>().enabled = true;
        carObject.GetComponent<Rigidbody>().useGravity = true;
        carObject.GetComponent<Rigidbody>().isKinematic = false;
        HeatAbility bAbility = GetComponent<HeatAbility>();
        AudioManager.instance.Play("Respawn complete", transform);
        if (bAbility != null)
        {
            bAbility.DeactivateAbility();
        }
        if (gameObject.GetComponent<VehicleInput>())
        {
            gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Clear();
        }
        //GetComponent<RaycastCar>().setDrifting(false);
       // GetComponent<RaycastCar>().SetIsBoosting(false);

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
            GetComponent<RaycastCar>().enabled = false;
            GetComponent<VehicleAbilityBehavior>().enabled = false;
            GameObject respawnInstance = Instantiate(respawnPlatform);
            respawnInstance.GetComponent<RespawnPlatformBehavior>().SetPlayer(this.gameObject, modelHolder);
            carObject.GetComponent<Rigidbody>().useGravity = false;
            carObject.GetComponent<Rigidbody>().isKinematic = true;
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

    public void DamageCar(float damage)
    {

        //stackTrace = new StackTrace();
       // print("ADDHEAT !! " + stackTrace.GetFrame(1).GetMethod().Name);
       if(_extraHP > 0)
        { //if you have _extraHP
            
            //damage left over after it's delt to the _extraHP
            float _tempDamage = damage - _extraHP;

            if(_tempDamage < 0)
            {//makes sure _tempDamage doesn't heal the player later on
                _tempDamage = 0;
            }

            _extraHP -= damage; //deals the damage to the _extraHP

            if(_extraHP <= 0)
            { //If you have no _extraHP left
                healthCurrent -= _tempDamage;
            }
        }
        else
        {
            healthCurrent -= damage;
            extra = -20;
        }
       
    }

    public void DamageCarTrue(float damage)
    {
        healthCurrent -= damage;
    }

    public float GetExtraHealth()
    {
        return _extraHP;
    }

    public void SetExtraHealth(float extraHP)
    {
        _extraHP = extraHP;
    }

    public void AddExtraHealth(float extraHP)
    {
        _extraHP += extraHP;
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


}

