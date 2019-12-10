using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_Projectile_Explosion : MonoBehaviour
{

    private float _explosionDamage;

    private float _explosionFuse;

    private float _explosionHypeToGain;

    private float _explosionRadius;

    Collider[] objectsHit;



    public void GiveInfo(float explosionDamage, float explosionFuse, float explosionHypeToGain, float explosionRadius)
    {
        _explosionDamage = explosionDamage;
        _explosionFuse = explosionFuse;
        _explosionHypeToGain = explosionHypeToGain;
        _explosionRadius = explosionRadius;
       // Debug.Log("Explosion Fuse at Explosion is: " + _explosionFuse);

    }


    public void LightFuse()
    {
        Invoke("BlowFuse", _explosionFuse);
    }

    public void BlowFuse()
    {
       objectsHit = Physics.OverlapSphere(gameObject.transform.localPosition, _explosionRadius);

        foreach (Collider obj in objectsHit)
        {
            Debug.Log("Object to be hit: " + obj.gameObject.name);
            if (obj.gameObject.GetComponent<CarHeatManager>() != null)
            {//if a car was hit
                obj.gameObject.GetComponent<CarHeatManager>().AddHeat(_explosionDamage);
                Debug.Log("Car was hit with explosion");

            }
            else if (obj.gameObject.GetComponent<Interactable>() != null)
            {
                obj.gameObject.GetComponent<Interactable>().DamageInteractable(_explosionDamage);
                Debug.Log("Interactable was hit with explosion");

            }
            else
            {
                Debug.Log("Ifs are definately fucking happening");
            }
        }
       // DestroyExplosion(); 
    }

    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }

}
