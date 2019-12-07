using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Display : MonoBehaviour
{

    public GameObject PlayerStatus;
    public GameObject activeStatus;
    public GameObject vehicle;
    private bool confirmation = true;
    private TextMeshProUGUI text;
    public GameObject playerManagement;
    public int PlayerNum;

    private void Start()
    {
        text = PlayerStatus.GetComponent<TextMeshProUGUI>();
    }
    public void changeStatus(bool status, int num = 0)
    {
        if (status == true)
        {
            text.text = "PLAYER " + num;
            activeStatus.gameObject.SetActive(false);
        }
        else if (status == false)
        {
            text.text = "NO PLAYER";
            activeStatus.gameObject.SetActive(true);
            confirmation = true;
            playerManagement.GetComponent<PlayerManagement>().ifReady();
            activeStatus.gameObject.GetComponent<TextMeshProUGUI>().text = "PRESS Y TO JOIN";
        }
    }

    public void addedCar(bool onOff)
    {
        if (onOff == true) {
            activeStatus.transform.localPosition = new Vector3(-90, 100, 0);
            activeStatus.gameObject.SetActive(true);
            activeStatus.gameObject.GetComponent<TextMeshProUGUI>().text = "PRESS A TO CONFIRM";
            vehicle.GetComponent<RawImage>().texture = null;
        }
        else if (onOff == false){
            activeStatus.transform.localPosition = new Vector3(0, 0, 0);
            confirmation = false;
            playerManagement.GetComponent<PlayerManagement>().ifReady();
            activeStatus.gameObject.SetActive(true);
            activeStatus.gameObject.GetComponent<TextMeshProUGUI>().text = "SELECT A VEHICLE";
        }
    }

    public void confirmedCar(bool confirm)
    {
        if (confirm == true)
        {
            activeStatus.GetComponent<TextMeshProUGUI>().text = "READY";
            confirmation = confirm;
            playerManagement.GetComponent<PlayerManagement>().ifReady();
        }
        else
        {
            confirmation = confirm;
            playerManagement.GetComponent<PlayerManagement>().ifReady();

        }

    }

    public bool getConfirmation()
    {
        return confirmation;
    }
}
