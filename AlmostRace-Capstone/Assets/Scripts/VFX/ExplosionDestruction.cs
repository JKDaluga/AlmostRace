using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDestruction : MonoBehaviour
{
    public float destroySecs = 3f;

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
