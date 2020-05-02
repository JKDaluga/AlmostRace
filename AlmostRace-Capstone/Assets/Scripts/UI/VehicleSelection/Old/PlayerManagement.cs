using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    Creator and developer of script: Leonardo Caballero
    Purpose: A system that keeps track of players that are currently playing, keeps track
    of what vehicle each player has chosen, and allows players to 
    turn each other off. Also helps display the correct text on the character select screen. 
*/

public class PlayerManagement : MonoBehaviour
{
    public GameObject startImage;
    public GameObject vehicles;
    private GameObject[] players = new GameObject[4];
    public GameObject[] players2 = new GameObject[4];
    private DataManager data;
    private static bool _isReady = false;
    private static bool _isLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        Display[] dis = FindObjectsOfType<Display>();
        for (int i = 0; i < dis.Length; i++)
        {
            players[i] = dis[i].gameObject;
        }
        _isLoading = false;
        data = DataManager.instance;
        data.ResetData();
    }

    private void FixedUpdate()
    {
        if(_isReady && !_isLoading && Input.GetButtonDown("Submit"))
        {
            _isLoading = true;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
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
            _isReady = true;
        }
        else
        {
            startImage.SetActive(false);
            vehicles.SetActive(true);
            _isReady = false;
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
        updatePlayerNums();
    }
    
    private void updatePlayerNums()
    {
        int i = 1;
        foreach(PlayerInfo player in data.playerInfo)
        {
            if(player.isActive)
            {
                player.playerID = i;
                i++;
            }
        }
    }
}
