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
    private GameObject pickUp;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            StartCoroutine(coolDown());
            if(collision.gameObject.GetComponent<VehicleAbilityBehavior>().hasPickup() == false)
            {
                if (chosenPickUp == PickUps.misslePickUp)
                {
                    pickUp = Resources.Load<GameObject>("MissileAbilityContainer");
                    collision.gameObject.GetComponent<VehicleAbilityBehavior>().assignPickup(pickUp);
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
