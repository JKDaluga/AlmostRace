using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinerendererGeneric : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform start;
    public Transform target;
    public bool usingExtraVFX = false;
    public bool realTimeVFX = false;
    public List<GameObject> particles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, target.position);
        if(usingExtraVFX)
        {
            foreach(GameObject vfx in particles)
            {
                vfx.SetActive(true);
                vfx.transform.position = lineRenderer.GetPosition(1);
            }
        }
        if(realTimeVFX)
        {
            StartCoroutine(UpdateVFX());
        }
    }

    public IEnumerator UpdateVFX()
    {
        while (true)
        {
            foreach (GameObject vfx in particles)
            {
                vfx.transform.position = lineRenderer.GetPosition(1);
            }
            yield return null;
        }


    }
}
