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
    public GameObject aggroObject;

    [Header("Combat Variables............................................................")]
    public GameObject currentTarget;
    public Transform turretHead;
    public Transform turretMuzzle;
    public float aimOffset = 2f;
    public float turretAimRate = .01f;
    public float turretFireRate = .5f;
    public float turretRespawnTime = 3f;
    public bool spraysBullets = true;
    public int extraBulletsToSpray = 5;

   
    [Header("Projectile Variables............................................................")]
    [Space(30)]
    public GameObject turretProjectile;
    public float turretProjectileDamage;
    public float turretProjectileSpeed;
   

  
    [Header("Fire Pillar Variables............................................................")]
    [Space(30)]
   // public TurretFirePillarBehavior turretFirePillar;

 
    [Header("Hype Variables............................................................")]
    [Space(30)]
    public float destroyTurretHype = 150f;
   // public float firePillarKillHype = 400f;
    
    
    [Header("Feedback Variables............................................................")]
    [Space(30)]
    public List<MeshRenderer> visibleMeshes;
    public ParticleSystem turretExplosionParticles;
    private AudioSource _turretSound;
   // public AudioClip turretFiringSound;
    public AudioClip turretExplosionSound;
    public ParticleSystem turretRespawnParticles;
    public AudioClip turretRespawnSound;

   private Vector3 m_lastKnownPosition = Vector3.zero;
   private Quaternion m_lookAtRotation;

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
        _turretCollider.enabled = false;//turn off collider to not block projectiles and vfx
        aggroObject.SetActive(false);
        CancelInvoke("AimTurret");//stop aiming
        CancelInvoke("FireTurret");//stop firing
        turretExplosionParticles.Play();//play explosion vfx
        foreach (MeshRenderer mesh in visibleMeshes)//visually disable turret || make it disappear
        {
            mesh.enabled = false;
        }
        _turretSound.PlayOneShot(turretExplosionSound); //play explosion sound
        currentTarget = null;//reset target
        //turretFirePillar.Activate(); //activate fire pillar
        interactingPlayer.GetComponent<VehicleHypeBehavior>().AddHype(destroyTurretHype, "Turret Destroyed");//award hype to interacting player
        Invoke("ResetInteractable", turretRespawnTime);
    }

    public override void ResetInteractable()
    {
        canBeDamaged = true;
        _turretCollider.enabled = true;//turn on collider
        aggroObject.SetActive(true);
        foreach (MeshRenderer mesh in visibleMeshes)//visually enable turret || make it disappear
        {
            mesh.enabled = true;
        }
        turretRespawnParticles.Play();
        //_turretSound.PlayOneShot(turretRespawnSound); //play respawn sound
       // turretFirePillar.Deactivate();//deactivate fire pillar
        interactableHealth = _originalHealth;//reset health
        CancelInvoke("AimTurret");//stop aiming
        CancelInvoke("FireTurret");//stop firing
    }

    public override void TriggerInteractable()
    {
        InvokeRepeating("AimTurret", 0, turretAimRate);//start aiming at target
        InvokeRepeating("FireTurret", 0, turretFireRate);//start shooting at target
    }

    public void AimTurret()
    {
        if(currentTarget != null)
        {

            /*if (m_lastKnownPosition != currentTarget.transform.position)
            {
                m_lastKnownPosition = currentTarget.transform.position;
                m_lookAtRotation = Quaternion.LookRotation(m_lastKnownPosition - turretHead.transform.position);
            }

            if (transform.rotation != m_lookAtRotation)
            {
                turretHead.transform.rotation = Quaternion.RotateTowards(turretHead.transform.rotation, m_lookAtRotation, 30 * Time.deltaTime);
            }*/

            Vector3 pos = predictedPosition(currentTarget.transform.position, turretMuzzle.position, currentTarget.GetComponent<SphereCarController>().sphere.velocity, turretProjectileSpeed);

            turretHead.LookAt(pos);//look at current target
        }
        else
        {
            CancelInvoke("AimTurret");//stop aiming
            CancelInvoke("FireTurret");//stop firing
        }
    }

    private Vector3 predictedPosition(Vector3 targetPosition, Vector3 shooterPosition, Vector3 targetVelocity, float projectileSpeed)
    {
        Vector3 displacement = targetPosition - shooterPosition;
        float targetMoveAngle = Vector3.Angle(-displacement, targetVelocity) * Mathf.Deg2Rad;
        //if the target is stopping or if it is impossible for the projectile to catch up with the target (Sine Formula)
        if (targetVelocity.magnitude == 0 || targetVelocity.magnitude > projectileSpeed && Mathf.Sin(targetMoveAngle) / projectileSpeed > Mathf.Cos(targetMoveAngle) / targetVelocity.magnitude)
        {
          //  Debug.Log("Position prediction is not feasible.");
            return targetPosition;
        }
        //also Sine Formula
        float shootAngle = Mathf.Asin(Mathf.Sin(targetMoveAngle) * targetVelocity.magnitude / projectileSpeed);
        return targetPosition + targetVelocity * displacement.magnitude / Mathf.Sin(Mathf.PI - targetMoveAngle - shootAngle) * Mathf.Sin(shootAngle) / targetVelocity.magnitude;
    }

    public void FireTurret()
    {
        //_turretSound.PlayOneShot(turretFiringSound);//play firing sound
        GameObject spawnedProjectile = Instantiate(turretProjectile, turretMuzzle.position, turretMuzzle.rotation);//fire projectile at current target
        spawnedProjectile.GetComponent<TurretProjectileBehavior>().SetProjectileInfo(turretProjectileDamage, turretProjectileSpeed, gameObject);
        if(spraysBullets)
        {

            for(int i = 0; i < extraBulletsToSpray; i++)
            {
                GameObject extraSpawnedProjectile = Instantiate(turretProjectile, turretMuzzle.position, turretMuzzle.rotation);//fire projectile at current target
                extraSpawnedProjectile.GetComponent<TurretProjectileBehavior>().SetProjectileInfo(turretProjectileDamage, turretProjectileSpeed, gameObject);
                extraSpawnedProjectile.transform.Rotate(Random.Range(-.5f, .5f), Random.Range(-2,2), Random.Range(-.5f, .5f));
            }
           


           

        }

    }
}
