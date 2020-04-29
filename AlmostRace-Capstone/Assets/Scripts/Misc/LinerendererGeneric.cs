using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinerendererGeneric : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform start;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, target.position);
    }
}
