using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B

    AI for the turrets.
     */

public class TurretBehavior : Interactable
{
    private Collider _turretCollider;
    private float _originalHealth;

    [Header("Combat Variables............................................................")]
    public GameObject currentTarget;
    public Transform turretHead;
    public Transform turretMuzzle;
    public float turretAimRate = .01f;
    public float turretFireRate = .5f;
    public float turretRespawnTime = 3f;

   
    [Header("Projectile Variables............................................................")]
    [Space(30)]
    public GameObject turretProjectile;
    public float turretProjectileDamage;
    public float turretProjectileSpeed;
   

  
    [Header("Fire Pillar Variables............................................................")]
    [Space(30)]
    public TurretFirePillarBehavior turretFirePillar;

 
    [Header("Hype Variables............................................................")]
    [Space(30)]
    public float destroyTurretHype = 150f;
    public float firePillarKillHype = 400f;
    
    
    [Header("Feedback Variables............................................................")]
    [Space(30)]
    public List<MeshRenderer> visibleMeshes;
    public ParticleSystem turretExplosionParticles;
    private AudioSource _turretSound;
    public AudioClip turretFiringSound;
    public AudioClip turretExplosionSound;
    public ParticleSystem turretRespawnParticles;
    public AudioClip turretRespawnSound;







    // Start is called before the first frame update
    void Start()
    {
        _turretCollider = gameObject.GetComponent<Collider>();
        _originalHealth = interactableHealth;
        canBeDamaged = true;

        _turretSound = gameObject.GetComponent<AudioSource>();
    }

    public override void DamageInteractable(float damageNumber)
    {
        if (canBeDamaged)
        {
            interactableHealth -= damageNumber;
            if (interactableHealth <= 0)
            {
                DestroyInteractable();
                if (interactingPlayer != null)
                {
                    interactingPlayer = null; // Resets interactingPlayer
                }
            }
        }
    }

    public override void DestroyInteractable()
    {
        canBeDamaged = false;
        _turretCollider.enabled = false;//turn off collider to not block projectiles
        CancelInvoke("AimTurret");//stop aiming
        CancelInvoke("FireTurret");//stop firing
        turretExplosionParticles.Play();//play explosion vfx
        foreach (MeshRenderer mesh in visibleMeshes)//visually disable turret || make it disappear
        {
            mesh.enabled = false;
        }
        _turretSound.PlayOneShot(turretExplosionSound); //play explosion sound
        currentTarget = null;//reset target
        turretFirePillar.Activate(); //activate fire pillar
        interactingPlayer.GetComponent<VehicleHypeBehavior>().AddHype(destroyTurretHype);//award hype to interacting player

    }

    public override void ResetInteractable()
    {
        canBeDamaged = true;
        _turretCollider.enabled = true;//turn on collider
        foreach (MeshRenderer mesh in visibleMeshes)//visually enable turret || make it disappear
        {
            mesh.enabled = false;
        }
        turretRespawnParticles.Play();
        _turretSound.PlayOneShot(turretRespawnSound); //play respawn sound
        turretFirePillar.Deactivate();//deactivate fire pillar
        interactableHealth = _originalHealth;//reset health
    }

    public override void TriggerInteractable()
    {
        //start aiming at target
        //start shooting at target
    }

    public void AimTurret()
    {
        turretHead.LookAt(currentTarget.transform.position);//look at current target
    }
    public void FireTurret()
    {
        _turretSound.PlayOneShot(turretFiringSound);//play firing sound
        GameObject spawnedProjectile = Instantiate(turretProjectile, turretMuzzle.position, turretMuzzle.rotation);//fire projectile at current target
    }
}
