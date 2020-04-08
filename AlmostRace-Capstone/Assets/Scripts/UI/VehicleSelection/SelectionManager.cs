using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
    public int amountOfSelections = 2;
    public string nextScene;
    public int mainMenuIndex = 0;
    public ViewportController[] viewports;
    public VehicleInput[] playerInputs;
    public GameObject allReadyImage;
    private DataManager _data;
    private bool _readyToStart = false;
    private bool _isLoading = false;


    // Start is called before the first frame update
    void Start()
    {
        _data = DataManager.instance;
        _isLoading = false;
        _data.resetData();
        allReadyImage.SetActive(false);
    }

    private void Update()
    {
        if (!_readyToStart && !_isLoading)
        {
            for (int i = 0; i < playerInputs.Length; i++)
            {
                if (Input.GetButtonDown(playerInputs[i].awakeButton))
                {
                    CheckController(playerInputs[i], "AwakeButtonTriggered");
                }
                if (Input.GetButtonDown(playerInputs[i].backButton))
                {
                    CheckController(playerInputs[i], "BackButtonTriggered");
                }
            }
        }

        if(_readyToStart && !_isLoading && Input.GetButtonDown("Submit"))
        {
            _isLoading = true;
            SceneManager.LoadSceneAsync(nextScene);
        }
    }

    private void CheckController(VehicleInput givenController, string givenCommand)
    {
        bool inUse = false;
        ViewportController selectedViewport = null;

        for (int i = 0; i < viewports.Length; i++)
        {
            if (viewports[i].GetJoined() && viewports[i].GetInput() == givenController)
            {
                inUse = true;
                selectedViewport = viewports[i];
                break;
            }
        }

        if (!inUse && givenCommand == "AwakeButtonTriggered")
        {
            AssignPlayer(givenController);
        }
        else if (givenCommand == "BackButtonTriggered")
        {
            if (inUse)
            {
                selectedViewport.PlayerJoin(false, null);
            }
            else if (!inUse)
            {
                _isLoading = true;
                SceneManager.LoadSceneAsync(mainMenuIndex);
            }
        }
    }

    private void AssignPlayer(VehicleInput givenController)
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
        _data.playerInfo[playerNumber - 1].isActive = isReady;
        _data.playerInfo[playerNumber - 1].carID = givenCarID;
        _data.playerInfo[playerNumber - 1].playerID = playerNumber;
        _data.playerInfo[playerNumber - 1].controllerID = givenControllerID;
        if (IsReady())
        {
            _readyToStart = true;
            allReadyImage.SetActive(true);
        }
    }

    private bool IsReady()
    {
        bool enoughPlayers = false;
        for(int i = 0; i < viewports.Length; i++)
        {
            if (viewports[i].GetJoined())
            {
                enoughPlayers = true;
                if (!viewports[i].GetReady())
                {
                    return false;
                }
            }
        }
        if (enoughPlayers)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetReadyToStart()
    {
        return _readyToStart;
    }
    
}
