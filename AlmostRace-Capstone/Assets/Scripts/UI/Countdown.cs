using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    Creator and developer of script: Leonardo Caballero
*/

public class Countdown : MonoBehaviour
{
    public int timeLeft = 3;
    private TextMeshProUGUI countText;
    private VehicleInput[] arrV;
    private bool startStatus = true;

    void Start()    
    {
        countText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(countDown(timeLeft));
    }
    public IEnumerator countDown(int seconds)
    {
        int count = seconds;

        while (count > 0)
        {
            countText.text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
            AudioManager.instance.Play("Countdown");
        }

        arrV = FindObjectsOfType<VehicleInput>();
        turnOff(true);
        startStatus = false;

        gameObject.SetActive(false);
        AudioManager.instance.Play("Countdown End");
    }

    private void turnOff(bool stat)
    {
        foreach (VehicleInput t in arrV)
        {
            t.setStatus(stat);
        }
    }
}
