using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialAudioTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SoundTest");
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        

    }

    IEnumerator SoundTest()
    {
      for(; ; )
        {
            AudioManager.instance.Play("Lux Shooting", transform);
            yield return new WaitForSeconds(1f);
        }
    }
}
