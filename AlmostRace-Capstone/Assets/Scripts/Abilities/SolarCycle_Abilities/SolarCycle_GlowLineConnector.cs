using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarCycle_GlowLineConnector : MonoBehaviour
{
    public LineRenderer lineRender;
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        lineRender.SetPosition(0, transform.position);
        lineRender.SetPosition(1, target.position);
    }
}
