using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    public enum PickUps{misslePickUp };
    public PickUps chosenPickUp;
    private GameObject pickUp;

    public MeshRenderer mesh;
    public BoxCollider collide;
    public int secs;

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
        collide.enabled = false;
        yield return new WaitForSeconds(secs);
        mesh.enabled = true;
        collide.enabled = true;
    }
}
