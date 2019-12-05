using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Mike Romeo
 *  
 *  Void Wasp boost ability
 */

public class VoidWasp_BoostAbility : CooldownHeatAbility
{
    private SphereCarController carInfo;

    [Range(0, 100)]
    public float boostSpeedPercentage;

    [Range(0, 100)]
    public float boostTopSpeedPercentage;

    public Animator carAnimation;

    [Header("Camera boost settings")]
    [Tooltip("Reference to the cinemachine camera object attached to the car")]
    public CinemachineVirtualCamera attachedCamera;

    public float cameraChangeDuration;
    public float cameraFovTarget;

    private float _defaultFov;

    private IEnumerator coChangeFoV;

    private void Start()
    {
        carInfo = gameObject.GetComponent<SphereCarController>();

        _defaultFov = attachedCamera.m_Lens.FieldOfView;
    }

    public override void ActivateAbility()
    {
        carInfo.SetIsBoosting(true);
        carInfo.SetBoostInfo(boostSpeedPercentage);

        carAnimation.SetBool("spinning", true);

        //ChangeFov(cameraChangeDuration, cameraFovTarget);

        gameObject.layer = 21;

        AddHeat();
    }

    public override void DeactivateAbility()
    {
        carInfo.SetIsBoosting(false);
        carAnimation.SetBool("spinning", false);

        gameObject.layer = 8;

        //Invoke("ResetFov", cameraChangeDuration);
    }

    protected override void AddHeat()
    {
        //throw new System.NotImplementedException();
    }

    /// <summary>
    /// Animate Field of view towards desired setting
    /// </summary>
    /// <param name="duration">Time is is going to take</param>
    /// <param name="value">What value to change it to</param>
    public void ChangeFov(float duration, float value)
    {
        if (coChangeFoV != null)
        {
            StopCoroutine(coChangeFoV);
        }
        coChangeFoV = CoChangeFoV(duration, value);
        StartCoroutine(coChangeFoV);
    }

    IEnumerator CoChangeFoV(float duration, float value)
    {
        float t = 0.0f;
        float startFoV = Camera.main.fieldOfView;
        while (t != duration)
        {
            t += Time.deltaTime;

            if (t > duration)
            {
                t = duration;
            }

            attachedCamera.m_Lens.FieldOfView = Mathf.Lerp(startFoV, value, t / duration);
            yield return null;
        }
    }

    private void ResetFov()
    {
        ChangeFov(cameraChangeDuration * 2, _defaultFov);
    }
}
