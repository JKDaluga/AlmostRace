using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : Interactable
{
    public float rotationTime;
    public float rotationAmount;
    public GameObject ramp;

    private bool rotating = false;
    bool wasRecentlyActivated = false;


    public override void DamageInteractable(float damageNumber)
    {
        //Unnecessary
    }

    public override void DestroyInteractable()
    {
        //Unnecessary
    }

    public override void ResetInteractable()
    {
        wasRecentlyActivated = false;

        StartCoroutine(RotateOverTime(ramp.transform.localEulerAngles.x, ramp.transform.localEulerAngles.x + (-rotationAmount), rotationTime));
        rotating = false;
    }

    public override void TriggerInteractable()
    {
        if (!rotating)
        {
            rotating = true;
            StartCoroutine(RotateOverTime(ramp.transform.localEulerAngles.x, ramp.transform.localEulerAngles.x + (rotationAmount), rotationTime));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ramp collided with: " + other.name);
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null && !wasRecentlyActivated)
        {
            Debug.Log("Ramp collided with car: " + other.name);
            interactingPlayer = other.gameObject;
            wasRecentlyActivated = true;
            TriggerInteractable();
        }
    }


    IEnumerator RotateOverTime(float currentRotation, float desiredRotation, float overTime)
    {
        float i = 0.0f;

        while (i < 1)
        {
            ramp.transform.localEulerAngles = new Vector3(Mathf.Lerp(currentRotation, desiredRotation, i), ramp.transform.localEulerAngles.y, ramp.transform.localEulerAngles.z);
            i += Time.deltaTime / overTime;
            yield return null;
        }


        yield return new WaitForSeconds(overTime);
        if (rotating == true)
        {
            ResetInteractable();
        }

    }
}
