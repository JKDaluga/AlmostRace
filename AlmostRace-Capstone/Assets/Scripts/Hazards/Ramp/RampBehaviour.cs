using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampBehaviour : Interactable
{

    public GameObject linkedButton;
    public float rotationTime;
    public float rotationAmount;
    GameObject ramp;

    bool rotating;




    // Start is called before the first frame update
    void Start()
    {
        ramp = this.gameObject;
        rotating = false;

    }


    public override void DamageInteractable(float damageNumber)
    {
        //Unnecessary
    }

    public override void DestroyInteractable()
    {
        //Unncessary
    }

    public override void ResetInteractable()
    {
        StartCoroutine(RotateOverTime(ramp.transform.localEulerAngles.x, ramp.transform.localEulerAngles.x + (-rotationAmount), rotationTime));
        rotating = false;
        linkedButton.GetComponent<ButtonBehaviour>().ResetInteractable();
    }

    public override void TriggerInteractable()
    {
        if (!rotating)
        {
            rotating = true;
            StartCoroutine(RotateOverTime(ramp.transform.localEulerAngles.x, ramp.transform.localEulerAngles.x + (rotationAmount), rotationTime));
        }
    }

    IEnumerator RotateOverTime(float currentRotation, float desiredRotation, float overTime)
    {
        float i = 0.0f;

        while (i < 1)
        {
            ramp.transform.localEulerAngles = new Vector3(Mathf.Lerp(currentRotation,desiredRotation,i), ramp.transform.localEulerAngles.y, ramp.transform.localEulerAngles.z);
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
