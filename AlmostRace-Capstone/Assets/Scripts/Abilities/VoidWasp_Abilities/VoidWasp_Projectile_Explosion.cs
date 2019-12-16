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

    private List<GameObject> objectsHit;

    private void Start()
    {
        gameObject.GetComponent<SphereCollider>().radius = _explosionRadius;
        objectsHit = new List<GameObject>();
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
        objectsHit.Add(hit.gameObject);

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
        Debug.Log("Blow Fuse");
        Debug.Log("List count: " + objectsHit.Count);

        foreach (GameObject obj in objectsHit)
        {
            Debug.Log("object name" + obj.gameObject.name);

            if (obj.gameObject != _immunePlayer)
            {
                if (obj.gameObject.GetComponent<CarHeatManager>() != null)
                {//if a car was hit
                    obj.gameObject.GetComponent<CarHeatManager>().DamageCar(_explosionDamage);


                    obj.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 4f;
                    obj.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_FrequencyGain = 4f;

                    obj.gameObject.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
                    //Debug.Log("Car was hit with explosion");

                }
                else if (obj.gameObject.GetComponent<Interactable>() != null)
                {
                    obj.gameObject.GetComponent<Interactable>().DamageInteractable(_explosionDamage);
                }
            }

        }
        Debug.Log("After for each loop");
        explosionParticles.SetActive(true);
        DestroyExplosion(explosionParticles.GetComponent<ParticleSystem>().main.startLifetime.constant);
    }

    public void DestroyExplosion(float time)
    {
        Destroy(gameObject, time);
    }

}
