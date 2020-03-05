using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamLock : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    private CinemachineOrbitalTransposer follow;
    private string input;
    public float range = 20;

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
            if (Mathf.Abs(follow.m_XAxis.Value) < 2)
            {
                follow.m_XAxis.m_MaxValue = 0;
                follow.m_XAxis.m_MinValue = 0;
            }
        }
        else
        {
            follow.m_XAxis.m_MaxValue = range;
            follow.m_XAxis.m_MinValue = -range;
        }
    }
}
