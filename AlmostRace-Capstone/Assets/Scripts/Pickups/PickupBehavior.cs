using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    public enum PickUps{misslePickUp};
    public PickUps chosenPickUp;
    public MeshRenderer mesh;
    public BoxCollider theCollider;
    public int respawnSeconds;
    private GameObject _pickUp;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            // Calls the cooldown on this pickup opject
            StartCoroutine(coolDown());

            // If the vehicle pickup slot is open assign a pickup
            if(collision.gameObject.GetComponent<VehicleAbilityBehavior>().hasPickup() == false)
            {
                if (chosenPickUp == PickUps.misslePickUp)
                {
                    _pickUp = Resources.Load<GameObject>("MissileAbilityContainer");
                    collision.gameObject.GetComponent<VehicleAbilityBehavior>().assignPickup(_pickUp);
                }
            }
        }
    }

    private IEnumerator coolDown()
    {
        mesh.enabled = false;
        theCollider.enabled = false;
        yield return new WaitForSeconds(respawnSeconds);
        mesh.enabled = true;
        theCollider.enabled = true;
    }
}
