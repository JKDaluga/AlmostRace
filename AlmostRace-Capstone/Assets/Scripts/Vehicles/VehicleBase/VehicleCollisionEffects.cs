using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VehicleCollisionEffects : MonoBehaviour
{

    public float velocityRequirement;
    public GameObject sparks;
    public Rigidbody colliderRigidbody;
    public GameObject sparkSoundObject;
    private AudioManager _audioManager;
    private AudioSource _audioSource;
    private bool _sparksPlaying;
    private bool _playAudio = true;

    private void Start()
    {
        if(AudioManager.instance != null)
        {
            _audioManager = AudioManager.instance;
        }
        else
        {
            _audioManager = GameObject.FindObjectOfType<AudioManager>();
        }
        if(_audioManager == null)
        {
            Debug.LogWarning("AudioManager Can Not be Found");
        }
        else
        {
            sparkSoundObject.transform.position = _audioManager.gameObject.transform.position;
        }
        _audioSource = sparkSoundObject.GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<VehicleInput>())
        {


           // //GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 1;
           // GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_FrequencyGain = 1;
           // GetComponent<CinemachineImpulseSource>().GenerateImpulse();
          //  AudioManager.instance.Play("General collision", transform);
            //CreateSparks(collision);
            _sparksPlaying = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (colliderRigidbody.velocity.magnitude > velocityRequirement)
        {
            //CreateSparks(collision);
            if (!_audioSource.isPlaying)
            {
              //  _audioSource.Play();
            }
            //foreach (ContactPoint contact in collision.contacts)
            //{
            //    Debug.DrawRay(contact.point, contact.normal, Color.white);
            //} Collision Debug
        }
        else
        {
            _audioSource.Stop();
            _sparksPlaying = false;
        }
    }

    void OnCollisionExit(Collision other)
    {
        _audioSource.Stop();
        _sparksPlaying = false;
    }

    public void CreateSparks(Collision givenCollision)
    {
        foreach (ContactPoint contact in givenCollision.contacts)
        {
            Instantiate(sparks, contact.point, Quaternion.identity);
        }  
    }

    public void toggleSparksSound(bool toggle)
    {
        _playAudio = toggle;
        if(_playAudio && _sparksPlaying)
        {
          //  _audioSource.Play();
        }
        else
        {
          //  _audioSource.Stop();
        }
    }
}
