using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
    public int amountOfSelections = 2;
    public string nextScene;
    private DataManager _data;
    public ViewportController[] viewports;
    private GameObject[] _players = new GameObject[4];
    public GameObject[] _players2 = new GameObject[4];
    private static bool _isReady = false;
    private static bool _isLoading = false;


    // Start is called before the first frame update
    void Start()
    {
        _data = DataManager.instance;
        for (int i = 0; i < viewports.Length; i++)
        {
            _players[i] = viewports[i].gameObject;
        }
        _isLoading = false;
        _data.resetData();
    }

    private void FixedUpdate()
    {
        if(_isReady && !_isLoading && Input.GetButtonDown("Submit"))
        {
            _isLoading = true;
            SceneManager.LoadSceneAsync(nextScene);
        }
    }

    public void UpdateReady()
    {
        /*
        int checker = 0;
        for(int i = 0; i != 4; i++)
        {
            if (_players[i].GetComponent<ViewportController>().GetReady())
            {
                checker++;
            }
        }
        if(checker == 4)
        {
            addData();
            _isReady = true;
        }
        else
        {
            _isReady = false;
        }
        */
    }

    private void addData()
    {
        for (int i = 0; i < 4; i++)
        {
            //_data.playerInfo[i].isActive = _players2[i].GetComponent<ViewportController>().GetReady();
           // _data.playerInfo[i].carID = _players2[i].GetComponent<ViewportController>().GetVehicle();
        }
        UpdatePlayerNums();
    }
    
    private void UpdatePlayerNums()
    {
        int i = 1;
        foreach(PlayerInfo player in _data.playerInfo)
        {
            if(player.isActive)
            {
                player.playerID = i;
                i++;
            }
        }
    }
}
