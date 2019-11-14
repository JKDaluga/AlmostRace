using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Creator and developer of script: Leonardo Caballero
*/

public class Countdown : MonoBehaviour
{
    public int timeLeft = 3;
    private Text countText;
    private VehicleInput[] arrV;
    private bool startStatus = true;

    void Start()    
    {
        countText = GetComponent<Text>();
        arrV = FindObjectsOfType<VehicleInput>();
        StartCoroutine(countDown(timeLeft));
    }
    IEnumerator countDown(int seconds)
    {
        int count = seconds;

        turnOff(false);

        while (count > 0)
        {
            countText.text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
        }

        turnOff(true);
        startStatus = false;

        gameObject.SetActive(false);
    }

    private void turnOff(bool stat)
    {
        foreach (VehicleInput t in arrV)
        {
            t.setStatus(stat);
        }
    }
}
