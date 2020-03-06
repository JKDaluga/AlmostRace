using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
    public int amountOfSelections = 2;
    public string nextScene;
    public ViewportController[] viewports;
    public VehicleInput[] playerInputs;
    public GameObject allReadyImage;
    private DataManager _data;
    private static bool _readyToStart = false;
    private static bool _isLoading = false;


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
        if (!_readyToStart)
        {
            for (int i = 0; i < playerInputs.Length; i++)
            {
                if (Input.GetButtonDown(playerInputs[i].awakeButton))
                {
                    CheckController(playerInputs[i]);
                }
            }
        }

        if(_readyToStart && !_isLoading && Input.GetButtonDown("Submit"))
        {
            _isLoading = true;
            SceneManager.LoadSceneAsync(nextScene);
        }
    }

    private void CheckController(VehicleInput givenController)
    {
        bool inUse = false;
        for (int i = 0; i < viewports.Length; i++)
        {
            if (viewports[i].GetJoined() && viewports[i].GetInput() == givenController)
            {
                inUse = true;
                break;
            }
        }
        if (!inUse)
        {
            AssignPlayer(givenController);
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

    public bool GetReadyToStart()
    {
        return _readyToStart;
    }
    
}
