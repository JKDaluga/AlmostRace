using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaPickupBehavior : MonoBehaviour
{
    private MeshRenderer gameObjectMesh;
    private Collider gameObjectCollider;
    public float pickupRecharge = 3f;

    public float animateSpeed = 0.01f;
    public float animateUpDownAmount = .01f;
    private int animateUpDownDirection = 1;
    public float animateSpinAmount = .01f;
    private float y = 0f;

    public float yMaxOffset = 1;
    public float yMinOffset = 0;
    private float yUpperLimit;
    private float yLowerLimit;

    public void Start()
    {
        gameObjectCollider = gameObject.GetComponent<Collider>();
        gameObjectMesh = gameObject.GetComponent<MeshRenderer>();

        yUpperLimit = gameObject.transform.localPosition.y + yMaxOffset;
       // Debug.Log(yUpperLimit);
        yLowerLimit = gameObject.transform.localPosition.y - yMinOffset;
       // Debug.Log(yLowerLimit);
        InvokeRepeating("ObjectBounce", 0 , animateSpeed);
    }



    public void FixedUpdate()
    {
        y += Time.deltaTime * animateSpinAmount;

        if (y > 360.0f)
        {
            y = 0.0f;
            
        }
        gameObject.transform.localRotation = Quaternion.Euler(45, y, 45);
    }


    public void ObjectBounce()
    {
        if (gameObject.transform.localPosition.y >= yUpperLimit || gameObject.transform.localPosition.y <= yLowerLimit)
        {
            animateUpDownDirection *= -1;
        }
        gameObject.transform.localPosition += new Vector3(0, animateUpDownAmount * animateUpDownDirection, 0);
    }

    public void ResetPickup()
    {
        gameObjectCollider.enabled = true;
        gameObjectMesh.enabled = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<WeaponSystemsManager>() != null)
        {
            other.gameObject.GetComponent<WeaponSystemsManager>().hasLavaPickup = true;
            gameObjectCollider.enabled = false;
            gameObjectMesh.enabled = false;
            Invoke("ResetPickup", pickupRecharge);
        }
    }
}
