using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypePopup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("HypeText Was Created");
    }

    public void DestroyText()
    {
        Destroy(gameObject);
    }
}
