using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Eddie Borissov
 Code that handles the functionality of the laser disks fired by the 
 Lux's offensive ability */

public class Lux_LaserDiskCode : Projectile
{
    private float _diskHype;


    /// <summary>
    /// might seem a bit extra just for one variable, but it makes more sense than making it 
    /// public and potentially having a designer accidentally fill it out here, or worse yet adding it to Projectile,
    /// which would make 0 sense, since most projectiles only have 1 hype variable (currently).
    /// </summary>
    /// <param name="diskHype"> The amount of hype you want to gain from scoring a direct hit </param>
    public void SetDiskHype(float diskHype)
    {
        _diskHype = diskHype;
    }
}
