using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creator and developer of script: Jake Velicer
    Purpose: Destroy object after [destroySecs] seconds
*/
public class DestroyAfterSeconds : MonoBehaviour
{
    public float destroySecs;
    
    void Start()
    {
        StartCoroutine(Destruction());
    }

    private IEnumerator Destruction()
    {
        yield return new WaitForSeconds(destroySecs);
        Destroy(gameObject);
    }
    
}
