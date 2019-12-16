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

    private List<GameObject> _hitObjects;

    private void Start()
    {
        gameObject.GetComponent<SphereCollider>().radius = _explosionRadius;
        _hitObjects = new List<GameObject>();
    }

    public void GiveInfo(float explosionDamage, float explosionFuse, float explosionHypeToGain, float explosionRadius, GameObject immunePlayer)
    {
        _explosionDamage = explosionDamage;
        _explosionFuse = explosionFuse;
        _explosionHypeToGain = explosionHypeToGain;
        _explosionRadius = explosionRadius;
        _immunePlayer = immunePlayer;
        // Debug.Log("Explosion Fuse at Explosion is: " + _explosionFuse);

    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.GetComponent<CarHeatManager>() != null)
        {
            _hitObjects.Add(hit.gameObject);
            hit.gameObject.GetComponent<CarHeatManager>().DamageCar(_explosionDamage);
           // Debug.Log("Car added: " + hit.gameObject.transform.parent.name);
        }
        else if (hit.gameObject.GetComponent<Interactable>() != null)
        {
            _hitObjects.Add(hit.gameObject);
            hit.gameObject.GetComponent<Interactable>().DamageInteractable(_explosionDamage);
           // Debug.Log("Interactable added: " + hit.gameObject.name);
        }
    }

    public void LightFuse()
    {
        Invoke("BlowFuse", _explosionFuse);

    }

    public void BlowFuse()
    {
        Transform parent = gameObject.transform.parent;
        gameObject.transform.SetParent(null);
        //objectsHit = Physics.OverlapSphere(gameObject.transform.localPosition, _explosionRadius);
        gameObject.transform.SetParent(parent);
     //   Debug.Log("Blow Fuse");
      //  Debug.Log("List count: " + _hitObjects.Count);

        foreach (GameObject obj in _hitObjects)
        {
           // Debug.Log("object name" + obj.gameObject.name + " in the list");

            if (obj.gameObject != _immunePlayer)
            {
                if (obj.gameObject.GetComponent<CarHeatManager>() != null)
                {//if a car was hit
                  //  obj.gameObject.GetComponent<CarHeatManager>().DamageCar(_explosionDamage);


                    obj.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 4f;
                    obj.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_FrequencyGain = 4f;

                    obj.gameObject.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
                    //Debug.Log("Car was hit with explosion");

                }
                else if (obj.gameObject.GetComponent<Interactable>() != null)
                {
                    //obj.gameObject.GetComponent<Interactable>().DamageInteractable(_explosionDamage);
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
