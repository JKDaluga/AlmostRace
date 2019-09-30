using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volt_ExplosionMissile : MonoBehaviour
{
    /*
     Edouard Borissov

        This simple script handles the growth of an explosion, as well as who it damages and how much.
         
         */
    private float growthRate = 0.01f;
    private float growthAmount = 0.1f;
    private float explosionLifeLength = 0.5f;
    private float explosionDamage;
    private float hypeAmount;
    private GameObject immunePlayer;


    void Start()
    {
        InvokeRepeating("Grow", 0, growthRate);
        Destroy(gameObject, explosionLifeLength);
    }

    public void Grow()
    {
        transform.localScale += new Vector3(growthAmount, growthAmount, growthAmount);
    }

    public void SetImmunePlayer(GameObject newImmunePlayer)
    {
        immunePlayer = newImmunePlayer;
    }

    public void SetExplosionDamage(float damage)
    {
        explosionDamage = damage;
    }

    public void SetHypeAmount(float hype)
    {
        hypeAmount = hype;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject != immunePlayer)
        {
            Debug.Log(explosionDamage + " damage was dealt to: " + collision.gameObject.name + " by a lightning missile explosion!");
            if (collision.GetComponent<CarHeatManager>() != null)
            {
                collision.GetComponent<CarHeatManager>().heatCurrent += explosionDamage;
                if (immunePlayer.GetComponent<VehicleHypeBehavior>() != null)
                {
                    immunePlayer.GetComponent<VehicleHypeBehavior>().AddHype(hypeAmount);
                }
            }
        }
        else if (collision.gameObject == immunePlayer)
        {
            Debug.Log("Player was detected by lightning explosion");
        }
    }
}
