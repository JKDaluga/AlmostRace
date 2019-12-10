using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Some edits by Leonardo Caballero
*/
public class CarHeatManager : MonoBehaviour
{
    public GameObject respawnPlatform;
    public GameObject modelHolder;
    public GameObject sphereCollider;
    public GameObject explosionEffect;
    public GameObject teleportEffect;
    public GameObject deathFade;
    public float healthCurrent = 0f;
    public float heatStallLimit = 100f;
    public float healthMax = 120f;
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
        if (Input.GetAxis(_vehicleInput.respawn) > 0 && _vehicleInput.getStatus() && !GetComponent<HotSpotVehicleAdministration>().holdingTheBot)
        {
            Teleport();
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

            if (healthCurrent < 0)
            {
                healthCurrent = 0;
            }

            if (healthCurrent == healthMax && gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }

            if (healthFillBar != null)
            {
                healthFillBar.fillAmount = healthCurrent/healthMax ;

                if (healthFillBar.fillAmount <= 0.25)
                {
                    if(!isFlashing)
                    {
                        isFlashing = true;
                        InvokeRepeating("FlashRedEngine", 0, engineFlashFreq);
                    }
                  
                }

                else if (healthFillBar.fillAmount < 0.5)
                {
                    engineImage.sprite = engineRed;                 
                }

                else if (healthFillBar.fillAmount < 0.75)
                {
                    engineImage.sprite = engineOrange;                 
                }

                else if (healthFillBar.fillAmount < 0.90)
                {
                    engineImage.sprite = engineYellow;             
                }
                else
                {
                    engineImage.sprite = engineGreen;
                    /*heat25.enabled = false;
                    heat50.enabled = false;
                    heat75.enabled = false;
                    heat90.enabled = false;*/
                }

            }
        }

    }

    private void FlashRedEngine()
    {
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
    }

    private void Kill()
    {
        //AudioManager.instance.Play("Death");
        isDead = true;
        Instantiate(explosionEffect, gameObject.transform.position, gameObject.transform.rotation);
        deathFade.GetComponent<Animator>().Play("DeathFadeIn");
        GetComponent<SphereCarController>().enabled = false;
        GetComponent<VehicleAbilityBehavior>().enabled = false;
        GameObject respawnInstance = Instantiate(respawnPlatform);
        respawnInstance.GetComponent<RespawnPlatformBehavior>().SetPlayer(this.gameObject, sphereCollider, modelHolder);
        sphereCollider.GetComponent<Rigidbody>().useGravity = false;
        sphereCollider.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Respawn()
    {
        isFlashing = false;
        CancelInvoke("FlashRedEngine");
        AudioManager.instance.Play("Respawn");
        healthCurrent = healthMax;
        isDead = false;
        deathFade.GetComponent<Animator>().Play("DeathFadeOut");
        GetComponent<SphereCarController>().enabled = true;
        GetComponent<VehicleAbilityBehavior>().enabled = true;
        sphereCollider.GetComponent<Rigidbody>().useGravity = true;
        sphereCollider.GetComponent<Rigidbody>().isKinematic = false;
        HeatAbility bAbility = GetComponent<HeatAbility>();
        if (bAbility != null)
        {
            bAbility.DeactivateAbility();
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
            AudioManager.instance.Play("Teleport");
            StartCoroutine(teleportCooldownTimer());
            Instantiate(teleportEffect, gameObject.transform.position, gameObject.transform.rotation);
            deathFade.GetComponent<Animator>().Play("DeathFadeIn");
            GetComponent<SphereCarController>().enabled = false;
            GetComponent<VehicleAbilityBehavior>().enabled = false;
            GameObject respawnInstance = Instantiate(respawnPlatform);
            respawnInstance.GetComponent<RespawnPlatformBehavior>().SetPlayer(this.gameObject, sphereCollider, modelHolder);
            sphereCollider.GetComponent<Rigidbody>().useGravity = false;
            sphereCollider.GetComponent<Rigidbody>().isKinematic = true;
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

    public void AddHeat(float heat)
    {
        healthCurrent -= heat;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("KillBox"))
        {
            Kill();
        }
    }
}

