using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Eddie Borissov
 Code that handles the functionality of the laser disks fired by the 
 Lux's offensive ability */

public class Lux_LaserDisk : Projectile
{
    private float _diskHype;

    private List<CarHealthBehavior> _carsDamaged = new List<CarHealthBehavior>();

    public Transform laserEmitterLeft;
    public Transform laserEmitterRight;

    private float _laserDamage;
    private float _laserDamageRate;

    private RaycastHit rayHit;

    public void FixedUpdate()
    {
        if (Physics.Raycast(laserEmitterLeft.position, laserEmitterLeft.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity))
        {
            if(rayHit.collider.gameObject.GetComponent<CarHealthBehavior>() != null)
            {//hit a car
                Debug.Log("Car found by laserEmitterLeft");
                _carsDamaged.Add(rayHit.collider.gameObject.GetComponent<CarHealthBehavior>());
            }
            Debug.DrawRay(laserEmitterLeft.position, laserEmitterLeft.TransformDirection(Vector3.forward) * rayHit.distance, Color.red);
           
        }

        if (Physics.Raycast(laserEmitterRight.position, laserEmitterRight.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity))
        {
            Debug.Log("Car found by laserEmitterRight");
            _carsDamaged.Add(rayHit.collider.gameObject.GetComponent<CarHealthBehavior>());
        }
        Debug.DrawRay(laserEmitterRight.position, laserEmitterRight.TransformDirection(Vector3.forward) * rayHit.distance, Color.red);

    }

    /// <summary>
    /// might seem a bit extra just for one variable, but it makes more sense than making it 
    /// public and potentially having a designer accidentally fill it out here, or worse yet adding it to Projectile,
    /// which would make 0 sense, since most projectiles only have 1 hype variable (currently).
    /// </summary>
    /// <param name="diskHype"> The amount of hype you want to gain from scoring a direct hit </param>
    public void SetDiskInfo(float laserDamage, float laserDamageRate, float diskHype)
    {
        _laserDamage = laserDamage;
        _diskHype = diskHype;
        _laserDamageRate = laserDamageRate;
    }

}
