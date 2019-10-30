using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{

    private int timeLeft;

    // Start is called before the first frame update
    void Start()    
    {
        StartCoroutine(countDown(3));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator countDown(int seconds)
    {
        int count = seconds;

        while (count > 0)
        {
            Debug.Log(count);
            // display something...
            yield return new WaitForSeconds(1);
            count--;
        }

        // count down is finished...
        //StartGame();
    }
}
