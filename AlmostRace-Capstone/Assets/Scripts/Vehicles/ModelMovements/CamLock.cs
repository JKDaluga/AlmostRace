using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamLock : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    private CinemachineOrbitalTransposer follow;
    private string input;
    public float range = 35;

    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        follow = cam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        input = follow.m_XAxis.m_InputAxisName;
    }

    private void Update()
    {
        if (Mathf.Abs(follow.m_XAxis.m_InputAxisValue) < .5f)
        {
            if (Mathf.Abs(follow.m_XAxis.Value) > .5f)
            {
                StartCoroutine(decreaseAxis(follow.m_XAxis.m_MaxValue));
            }
        }
        else
        {
            follow.m_XAxis.m_MaxValue = range;
            follow.m_XAxis.m_MinValue = -range;
        }
    }

    IEnumerator decreaseAxis(float oldValue)
    {
        for (float t = 0f; t < follow.m_RecenterToTargetHeading.m_RecenteringTime; t += Time.deltaTime)
        {
            follow.m_XAxis.m_MaxValue = Mathf.Lerp(oldValue, 0, t / follow.m_RecenterToTargetHeading.m_RecenteringTime);
            follow.m_XAxis.m_MinValue = -follow.m_XAxis.m_MaxValue;
            yield return null;
        }

    }
}
