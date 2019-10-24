using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearMiss : MonoBehaviour
{
    public List<GameObject> nearHits = new List<GameObject>();
    public List<GameObject> hits = new List<GameObject>();
    public LayerMask missable;
    public VehicleHypeBehavior hype;

    private void FixedUpdate()
    {

        RaycastHit[] closecall;

        closecall = Physics.SphereCastAll(transform.position, 5, transform.position, 10, missable);

        Debug.Log(closecall.Length);

        for (int i = 0; i < closecall.Length; i++)
        {
            if(!nearHits.Find(GameObject => GameObject == closecall[i].collider.gameObject) && !hits.Find(GameObject => GameObject == closecall[i].collider.gameObject))
            {
                nearHits.Add(closecall[i].collider.gameObject);
            }
        }

        foreach (GameObject target in nearHits)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > 12)
            {
                nearHits.Remove(target);
                if (!hits.Find(GameObject => GameObject == target))
                {
                    hype.AddHype(5.0f);
                }
                else
                {
                    hits.Remove(target);
                }
            }
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(nearHits.Find(GameObject => GameObject == collision.gameObject))
        {
            hits.Add(collision.gameObject);
        }
    }
}
