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
    private static bool _readyToStart = false;
    private static bool _isLoading = false;
    public PlayerInput[] _playerInputs;


    // Start is called before the first frame update
    void Start()
    {
        _data = DataManager.instance;
        _isLoading = false;
        _data.resetData();
    }

    private void Update()
    {
        for (int i = 0; i < _playerInputs.Length; i++)
        {
            if (Input.GetButtonDown(_playerInputs[i].awakeButton))
            {
                AssignPlayer(_playerInputs[i]);
            }
        }

        if(_readyToStart && !_isLoading && Input.GetButtonDown("Submit"))
        {
            _isLoading = true;
            SceneManager.LoadSceneAsync(nextScene);
        }
    }

    private void AssignPlayer(PlayerInput givenController)
    {
        for (int i = 0; i < viewports.Length; i++)
        {
            if (!viewports[i].GetJoined())
            {
                viewports[i].PlayerJoin(true, givenController);
                break;
            }
        }
    }

    public void UpdateData(int playerNumber, bool isReady, int givenCarID, int givenControllerID)
    {
        _data.playerInfo[playerNumber].isActive = isReady;
        _data.playerInfo[playerNumber].carID = givenCarID;
        _data.playerInfo[playerNumber].playerID = playerNumber;
        _data.playerInfo[playerNumber].controllerID = givenControllerID;
        if (IsReady())
        {
            _readyToStart = true;
            //Put UI stuff here
        }
    }

    private bool IsReady()
    {
        for(int i = 0; i < viewports.Length; i++)
        {
            if (viewports[i].GetJoined())
            {
                if (!viewports[i].GetReady())
                {
                    return false;
                }
            }
        }
        return true;
    }
    
}
