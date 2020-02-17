using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutEventExample : Shortcut
{
    [Tooltip("The time to wait to turn off the barrier after the explosion is triggered")] public float waitToOpen = 0.1f;
    public GameObject[] rocks;
    public GameObject[] particles;

    public override void ActivateAbility()
    {
        StartCoroutine(RockExplosion());
    }

    private IEnumerator RockExplosion()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].SetActive(true);
        }
        yield return new WaitForSeconds(waitToOpen);
        for (int i = 0; i < rocks.Length; i++)
        {
            rocks[i].SetActive(false);
        }
    }
}
