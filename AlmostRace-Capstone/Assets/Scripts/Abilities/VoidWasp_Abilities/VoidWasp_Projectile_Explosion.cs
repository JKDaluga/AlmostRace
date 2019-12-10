using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_Projectile_Explosion : MonoBehaviour
{

    private float _explosionDamage;

    private float _explosionFuse;

    private float _explosionHypeToGain;

    private float _explosionRadius;

    RaycastHit[] objectsHit;

    public void GiveInfo(float explosionDamage, float explosionFuse, float explosionHypeToGain, float explosionRadius)
    {
        _explosionDamage = explosionDamage;
        _explosionFuse = explosionFuse;
        _explosionHypeToGain = explosionHypeToGain;
        _explosionRadius = explosionRadius;
        LightFuse();
    }


    public void LightFuse()
    {
        Invoke("BlowFuse", _explosionFuse);
    }

    public void BlowFuse()
    {
       objectsHit = Physics.SphereCastAll(gameObject.transform.position, _explosionRadius, Vector3.zero);

        foreach(RaycastHit obj in objectsHit)
        {
            if (obj.transform.gameObject.GetComponent<CarHeatManager>() != null)
            {//if a car was hit
                obj.transform.gameObject.GetComponent<CarHeatManager>().AddHeat(_explosionDamage);
            }

            if(obj.transform.gameObject.GetComponent<Interactable>() != null)
            {
                obj.transform.gameObject.GetComponent<Interactable>().DamageInteractable(_explosionDamage);
            }
        }
    }

    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }

}
