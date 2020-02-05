using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_Attack : Ability
{
    [Header("Ability Values")]
    [Space(30)]
    public GameObject voidwaspProjectile;
    private bool _canFire = true;
    private float _cooldownTime = 0;
    [Tooltip("How quickly each projectile moves.")] public float projectileSpeed;
    [Tooltip("How many projectile it shoots in total")] public int projectileCount = 8;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ActivateAbility()
    {
        if (_canFire)
        {
            
        }
    }

    public override void DeactivateAbility()
    {

    }
}
