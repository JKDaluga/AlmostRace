using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Some edits by Leonardo Caballero
*/
public class CarHeatManager : MonoBehaviour
{
    public Image heatImage;
    public GameObject respawnPlatform;
    public GameObject modelHolder;
    public GameObject sphereCollider;
    public GameObject explosionEffect;
    public GameObject teleportEffect;
    public GameObject deathFade;
    public float heatCurrent = 0f;
    public float heatStallLimit = 100f;
    public float heatExplodeLimit = 120f;
    public float cooldownAmount = 1f;
    public float respawnSecs = 3f;
    public float cooldownFrequency = 2f;
    public bool isDead;
    private VehicleInput _vehicleInput;
    private bool _canTeleport = true;

    [Header("Car Heat UI elements")]
    public Image heatFillBar;
    public Image heat25;
    public Image heat50;
    public Image heat75;
    public Image heat90;


    private void Start()
    {
        _vehicleInput = GetComponent<VehicleInput>();
        InvokeRepeating("healthCooldown", 0, cooldownFrequency);

        heat25.enabled = false;
        heat50.enabled = false; 
        heat75.enabled = false; 
        heat90.enabled = false; 

    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis(_vehicleInput.respawn) > 0 && _vehicleInput.getStatus() && !GetComponent<HotSpotVehicleAdministration>().holdingTheBot)
        {
            Teleport();
        }

        if (!isDead)
        {
            if (heatCurrent > heatExplodeLimit)
            {
                heatCurrent = heatExplodeLimit;
            }

            if (heatCurrent >= heatExplodeLimit)
            {
                Kill();
            }

            if (heatCurrent < 0)
            {
                heatCurrent = 0;
            }

            if (heatCurrent == 0 && gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }

            if (heatImage != null)
            {
                //heatImage.fillAmount = ((heatCurrent * 100) / 120) / 100;
                heatFillBar.fillAmount = ((heatCurrent * 100) / 120) / 100;

                if (heatFillBar.fillAmount >= 0)
                {
                    heat25.enabled = false;
                    heat50.enabled = false;
                    heat75.enabled = false;
                    heat90.enabled = false;
                }

                if (heatFillBar.fillAmount > 0.25)
                {
                    heat25.enabled = true;
                    heat50.enabled = false;
                    heat75.enabled = false;
                    heat90.enabled = false;
                }

                if (heatFillBar.fillAmount > 0.5)
                {
                    heat25.enabled = true;
                    heat50.enabled = true;
                    heat75.enabled = false;
                    heat90.enabled = false;
                }

                if (heatFillBar.fillAmount > 0.75)
                {
                    heat25.enabled = true;
                    heat50.enabled = true;
                    heat75.enabled = true;
                    heat90.enabled = false;
                }

                if (heatFillBar.fillAmount > 0.90)
                {
                    heat25.enabled = true;
                    heat50.enabled = true;
                    heat75.enabled = true;
                    heat90.enabled = true;
                }
                else
                {
                    /*heat25.enabled = false;
                    heat50.enabled = false;
                    heat75.enabled = false;
                    heat90.enabled = false;*/
                }

            }
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
        _canTeleport = true;
        heatCurrent = 0;
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
            AudioManager.instance.Play("Teleport");
            _canTeleport = false;
            isDead = true;
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

    private void healthCooldown()
    {
        heatCurrent -= cooldownAmount;
    }

    public void AddHeat(float heat)
    {
        heatCurrent += heat;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("KillBox"))
        {
            Kill();
        }
    }
}

