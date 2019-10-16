using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CarHeatManager : MonoBehaviour
{
    public Image heatImage;
    public GameObject respawnPlatform;
    public GameObject modelHolder;
    public GameObject sphereCollider;
    public GameObject DeathFade;
    public float heatCurrent = 0f;
    public float heatStallLimit = 100f;
    public float heatExplodeLimit = 120f;
    public float cooldownAmount = 1f;
    public float respawnSecs = 3f;
    public float cooldownFrequency = 2f;
    private bool _isDead;
    private VehicleInput _vehicleInput;
    private bool _canTeleport = true;

    private void Start()
    {
        _vehicleInput = GetComponent<VehicleInput>();
        InvokeRepeating("healthCooldown", 0, cooldownFrequency);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))//for testing, heats up car
        {
            heatCurrent = 110f;
        }
        else if (Input.GetKeyDown(KeyCode.N)) // for testing, cools down car
        {
            heatCurrent = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            heatCurrent = 120f;
        }

        if(Input.GetAxis(_vehicleInput.respawn) > 0)
        {
            Debug.Log("Respawn should have happened");
            Teleport();
        }

        if (!_isDead)
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
                heatImage.fillAmount = ((heatCurrent * 100) / 120) / 100;
            }
        }

    }


    private void Kill()
    {
        _isDead = true;
        Instantiate(Resources.Load("explosion"), gameObject.transform.position, gameObject.transform.rotation);
        DeathFade.GetComponent<Animator>().Play("DeathFadeIn");
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
        _isDead = false;
        DeathFade.GetComponent<Animator>().Play("DeathFadeOut");
        GetComponent<SphereCarController>().enabled = true;
        GetComponent<VehicleAbilityBehavior>().enabled = true;
        sphereCollider.GetComponent<Rigidbody>().useGravity = true;
        sphereCollider.GetComponent<Rigidbody>().isKinematic = false;
        BasicAbility bAbility = GetComponent<BasicAbility>();
        if (bAbility != null)
        {
            bAbility.DeactivateAbility();
        }

    }

    public void Teleport()
    {
        if(_canTeleport)
        {
            _canTeleport = false;
            _isDead = true;
            Instantiate(Resources.Load("Teleport"), gameObject.transform.position, gameObject.transform.rotation);
            DeathFade.GetComponent<Animator>().Play("DeathFadeIn");
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

