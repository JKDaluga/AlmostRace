using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VoidWasp_Projectile_Explosion : MonoBehaviour
{

    private float _explosionDamage;

    private float _explosionFuse;

    private float _explosionHypeToGain;

    private float _explosionRadius;

    private GameObject _immunePlayer;

    public GameObject explosionParticles;

    Collider[] objectsHit;



    public void GiveInfo(float explosionDamage, float explosionFuse, float explosionHypeToGain, float explosionRadius, GameObject immunePlayer)
    {
        _explosionDamage = explosionDamage;
        _explosionFuse = explosionFuse;
        _explosionHypeToGain = explosionHypeToGain;
        _explosionRadius = explosionRadius;
        _immunePlayer = immunePlayer;
       // Debug.Log("Explosion Fuse at Explosion is: " + _explosionFuse);

    }


    public void LightFuse()
    {
        Invoke("BlowFuse", _explosionFuse);
    }

    public void BlowFuse()
    {
        Transform parent = gameObject.transform.parent;
        gameObject.transform.SetParent(null);
        objectsHit = Physics.OverlapSphere(gameObject.transform.localPosition, _explosionRadius);
        gameObject.transform.SetParent(parent);
        foreach (Collider obj in objectsHit)
        {
            if(obj.gameObject != _immunePlayer)
            {
                if (obj.gameObject.GetComponent<CarHeatManager>() != null)
                {//if a car was hit
                    obj.gameObject.GetComponent<CarHeatManager>().AddHeat(_explosionDamage);


                    obj.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = .25f;
                    obj.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_FrequencyGain = .25f;

                    obj.gameObject.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
                    //Debug.Log("Car was hit with explosion");

                }
                else if (obj.gameObject.GetComponent<Interactable>() != null)
                {
                    obj.gameObject.GetComponent<Interactable>().DamageInteractable(_explosionDamage);
                    if (obj.gameObject.GetComponent<Interactable>().interactableHealth <= 0)
                    {
                        if (_immunePlayer.GetComponent<AimAssistant>().target == obj.gameObject)
                        {
                            _immunePlayer.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Remove(obj.gameObject);
                        }
                    }

                    // Debug.Log("Interactable was hit with explosion");

                }
            }

        }
        explosionParticles.SetActive(true);
        DestroyExplosion(explosionParticles.GetComponent<ParticleSystem>().main.startLifetime.constant); 
    }

    public void DestroyExplosion(float time)
    {
        Destroy(gameObject, time);
    }

}
