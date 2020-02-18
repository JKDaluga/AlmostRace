using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_Attack : Ability
{
    [Header("Ability Values")]
    [Space(5)]
    public GameObject voidwaspProjectile;
    private float _cooldownTime = 0;
    [Tooltip("How quickly each projectile moves.")] public float projectileSpeed;
    [Tooltip("How many projectile it shoots in total")] public int projectileCount = 8;
    private List<GameObject> _objectsInRange = new List<GameObject>();

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
        if (_objectsInRange.Count > 0)
        {
            for (int i = 0; i < _objectsInRange.Count; i++)
            {
                if (_objectsInRange[i] == null)
                {
                    _objectsInRange.RemoveAt(i);
                }
            }
        }
    }

    public override void DeactivateAbility()
    {

    }

    public void AddObjectInRange(GameObject objectToAdd)
    {
        _objectsInRange.Add(objectToAdd);
    }

    public void RemoveObjectInRange(GameObject objectToRemove)
    {
        for(int i = _objectsInRange.Count - 1; i >= 0; i--)
        {
            if(_objectsInRange[i] == objectToRemove)
            {
                _objectsInRange.RemoveAt(i);
            }
        }
    }
}
