using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypePopup : MonoBehaviour
{
    public float lifeTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("HypeText Was Created");
        Destroy(gameObject, lifeTime);
    }
}
