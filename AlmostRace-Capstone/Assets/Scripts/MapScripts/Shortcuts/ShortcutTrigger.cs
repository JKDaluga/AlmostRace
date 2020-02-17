using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutTrigger : MonoBehaviour
{
    public Shortcut[] shortcuts;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>())
        {
            for (int i = 0; i < shortcuts.Length; i++)
            {
                shortcuts[i].ActivateAbility();
            }
        }
    }
}
