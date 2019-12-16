using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Author: Eddie Borrisov
 Purpose: Assigned boulder gravity based on size
 *LEGACY CODE*
*/
public class BoulderSuperGravity : MonoBehaviour
{
    public float downForce = 100;
    public float lifeTime = 10;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.down * downForce, ForceMode.Impulse);
        Destroy(gameObject,lifeTime);
    }
    private void FixedUpdate()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.down * downForce, ForceMode.Impulse);
    }
}
