using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B

    AI for the turrets.
     */

public class TurretBehavior2 : Interactable
{
    private Collider _turretCollider;
    private float _originalHealth;
    public GameObject aggroObject;

    [Header("Combat Variables............................................................")]
    public bool hasPower = false;
    public GameObject currentTarget;
    public List<GameObject> possibleTargets;
    public Transform turretHead;
    public Transform turretMuzzle;
    public float aimOffset = 2f;
    public float turretAimRate = .01f;
    public float turretFireRate = .5f;
    public float turretResetTime = 3f;
    public bool spraysBullets = true;
    public int extraBulletsToSpray = 5;


    [Header("Projectile Variables............................................................")]
    [Space(30)]
    public GameObject turretProjectile;
    public float turretProjectileDamage;
    public float turretProjectileSpeed;



    //[Header("Fire Pillar Variables............................................................")]
    //[Space(30)]
    //public TurretFirePillarBehavior turretFirePillar;


    [Header("Hype Variables............................................................")]
    [Space(30)]
    public float tagTurretHype = 50f;


    [Header("Feedback Variables............................................................")]
    [Space(30)]
    public List<MeshRenderer> turretGlows;
    public Material deactivatedMaterial;
    public Material p1Glow;
    public Material p2Glow;
    public Material p3Glow;
    public Material p4Glow;
    private AudioSource _turretSound;
    // public AudioClip turretFiringSound;

    // Start is called before the first frame update
    void Start()
    {
        //_turretCollider = gameObject.GetComponent<Collider>();
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
                canBeDamaged = false;
                Debug.Log("Turret has been killed");
                hasPower = true;
                TriggerInteractable();
                int interactingPlayerInput = interactingPlayer.GetComponent<VehicleInput>().getPlayerNum();
                switch (interactingPlayerInput)
                {
                    case 1:
                        //if P1 is detected
                        Debug.Log("p1 detected");
                        foreach (MeshRenderer mesh in turretGlows)//visually disable turret || make it disappear
                        {
                            mesh.material = p1Glow;
                        }
                        break;
                    case 2:
                        //if P2 is detected
                        Debug.Log("p2 detected");
                        foreach (MeshRenderer mesh in turretGlows)//visually disable turret || make it disappear
                        {
                            mesh.material = p2Glow;
                        }
                        break;
                    case 3:
                        //if P3 is detected
                        Debug.Log("p3 detected");
                        foreach (MeshRenderer mesh in turretGlows)//visually disable turret || make it disappear
                        {
                            mesh.material = p3Glow;
                        }
                        break;
                    case 4:
                        //if P4 is detected
                        Debug.Log("p4 detected");
                        foreach (MeshRenderer mesh in turretGlows)//visually disable turret || make it disappear
                        {
                            mesh.material = p4Glow;
                        }
                        break;
                }
                DestroyInteractable();

            }
        }
    }

    public override void DestroyInteractable()
    {
        interactingPlayer.GetComponent<VehicleHypeBehavior>().AddHype(tagTurretHype, "Turret Tag");//award hype to interacting player
        Invoke("ResetInteractable", turretResetTime);
    }

    public override void ResetInteractable()
    {
      //  Debug.Log("Turret was reset!");
        currentTarget = null;
        canBeDamaged = true;
        hasPower = false;
        foreach (MeshRenderer mesh in turretGlows)//visually disable turret || make it disappear
        {
            mesh.material = deactivatedMaterial;
        }
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
        if (hasPower)
        {
            if (currentTarget == null)
            {
                FindTarget();
            }
            else if (currentTarget != null)
            {
                turretHead.LookAt(currentTarget.transform.position + (aimOffset * currentTarget.GetComponent<SphereCarController>().sphere.velocity));//look at current target
            }
        }
        else
        {
            CancelInvoke("AimTurret");//stop aiming
            CancelInvoke("FireTurret");//stop firing
        }

    }


    public void FireTurret()
    {
        if (hasPower)
        {
            if (currentTarget == null)
            {
                FindTarget();
            }
            else
            {
                GameObject spawnedProjectile = Instantiate(turretProjectile, turretMuzzle.position, turretMuzzle.rotation);//fire projectile at current target
                spawnedProjectile.GetComponent<TurretProjectileBehavior>().SetProjectileInfo(turretProjectileDamage, turretProjectileSpeed, gameObject);
                if (spraysBullets)
                {

                    for (int i = 0; i < extraBulletsToSpray; i++)
                    {
                        GameObject extraSpawnedProjectile = Instantiate(turretProjectile, turretMuzzle.position, turretMuzzle.rotation);//fire projectile at current target
                        extraSpawnedProjectile.GetComponent<TurretProjectileBehavior>().SetProjectileInfo(turretProjectileDamage, turretProjectileSpeed, gameObject);
                        extraSpawnedProjectile.transform.Rotate(Random.Range(-.5f, .5f), Random.Range(-2, 2), Random.Range(-.5f, .5f));
                    }
                }
                //_turretSound.PlayOneShot(turretFiringSound);//play firing sound
            }
        }
        else
        {
            CancelInvoke("AimTurret");//stop aiming
            CancelInvoke("FireTurret");//stop firing
        }
    }

    public void FindTarget()
    {
        //Debug.Log("null target!");
        for (int i = 0; i < possibleTargets.Count; i++)
        {//Searches through possible targets, finds the first one that ISN'T the one that activated the turret
            if (possibleTargets[i].GetComponent<CarHealthBehavior>().isDead)
            {
               // Debug.Log("Dead player detected!");
                possibleTargets.Remove(possibleTargets[i]); //should clean List of dead targets.
                if (possibleTargets.Count == 0)
                {
                    Debug.Log("Turret found no targets, powering down!");
                    ResetInteractable(); //turns off turret if no possibleTargets
                }
            }
            else
            {
                if (possibleTargets[i] != interactingPlayer)
                {
                  //  Debug.Log("TARGET FOUND!");
                    currentTarget = possibleTargets[i]; // finds suitable target and sets it to the current
                    i = 100;
                }
                else
                {
                 //   Debug.Log("unsuitable target, going to next!");
                }
            }

        }
    }


}
