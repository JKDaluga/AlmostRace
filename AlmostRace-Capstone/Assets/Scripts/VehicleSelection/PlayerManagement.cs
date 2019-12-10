using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManagement : MonoBehaviour
{
    public GameObject startImage;
    public GameObject vehicles;
    private GameObject[] players = new GameObject[4];
    public GameObject[] players2 = new GameObject[4];
    private DataManager data;



    // Start is called before the first frame update
    void Start()
    {
        Display[] dis = FindObjectsOfType<Display>();
        for (int i = 0; i < dis.Length; i++)
        {
            players[i] = dis[i].gameObject;
        }

        data = DataManager.instance;
        data.resetData();
    }

    public void turnOff(int player)
    {
        if (players2[player].gameObject.GetComponent<VirtualMouse>() != null)
        {
        
            players2[player].gameObject.GetComponentInChildren<VirtualMouse>()._mouse.SetActive(false);
            players2[player].gameObject.GetComponentInChildren<VirtualMouse>().currentVehicle = 0;
            players2[player].gameObject.GetComponent<VirtualMouse>().changeReady(false);
            players2[player].gameObject.GetComponent<VirtualMouse>().Grid.GetComponent<Display>().addedCar(false);
            players2[player].gameObject.GetComponent<VirtualMouse>().Grid.GetComponent<Display>().changeStatus(false);
        }
    }

    public void ifReady()
    {
        int checker = 0;
        for(int i = 0; i != 4; i++)
        {
            if (players[i].GetComponent<Display>().getConfirmation() == true )
            {
                checker++;
            }
        }
        if(checker == 4 && players[0].GetComponent<Display>().PlayerStatus.GetComponent<TextMeshProUGUI>().text != "NO PLAYER")
        {
            startImage.SetActive(true);
            vehicles.SetActive(false);
            addData();
        }
        else
        {
            startImage.SetActive(false);
            vehicles.SetActive(true);
        }
    }

    private void addData()
    {

        for (int i = 0; i < 4; i++)
        {
            data.playerInfo[i].isActive = players2[i].GetComponent<VirtualMouse>().checkReady();
            data.playerInfo[i].carID = players2[i].GetComponent<VirtualMouse>().currentVehicle;
            Debug.Log(data.playerInfo[i].isActive);
        }

        
    }
    
}
