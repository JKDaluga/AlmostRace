using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimCollider : MonoBehaviour
{
    public AimAssistant aim;

    public List<GameObject> colliding = new List<GameObject>();

    float shortest;

    private void Start()
    {
        shortest = Mathf.Infinity;
    }

    private void FixedUpdate()
    {
        if (colliding.Count > 0)
        {
            foreach (GameObject i in colliding)
            {
                if (Vector3.Distance(aim.gameObject.transform.position, i.transform.position) < shortest)
                {
                    shortest = Vector3.Distance(aim.gameObject.transform.position, i.transform.position);
                    aim.nearest = i;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        if(aim.shootable == (aim.shootable | ( 1 << other.gameObject.layer)) && other.gameObject != aim.self)
        {
            colliding.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (colliding.Contains(other.gameObject))
        {
            colliding.Remove(other.gameObject);
            if(colliding.Count == 0)
            {
                shortest = Mathf.Infinity;
            }
            if(aim.nearest == other.gameObject)
            {
                aim.nearest = null;
            }
        }
    }

}
