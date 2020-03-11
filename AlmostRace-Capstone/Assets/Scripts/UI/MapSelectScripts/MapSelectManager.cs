using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MapSelectManager : MonoBehaviour
{
    public string vehicleSelectScene;
    public int mineMapScene = 3;
    public int stellarMapScene = 4;
    public GameObject[] playerLayouts;
    public MapSelectViewport[] viewports;
    public VehicleInput[] playerInputs;
    public TextMeshProUGUI _mineMapVoteText;
    public TextMeshProUGUI _stellarMapVoteText;
    public GameObject allReadyImage;
    private DataManager _data;
    private bool _readyToStart = false;
    private bool _isLoading = false;
    private int mineMapVotes = 0;
    private int stellarMapVotes = 0;


    // Start is called before the first frame update
    void Start()
    {
        _data = DataManager.instance;
        _isLoading = false;

        foreach(GameObject layout in playerLayouts)
        {
            layout.SetActive(false);
        }

        int numPlayers = _data.getNumActivePlayers();
        switch (numPlayers)
        {
            case 1:
                viewports = new MapSelectViewport[1];
                break;
            case 2:
                viewports = new MapSelectViewport[2];
                break;
            case 3:
                viewports = new MapSelectViewport[3];
                break;
            case 4:
                viewports = new MapSelectViewport[4];
                break;
            default:
                break;
        }
        viewports = playerLayouts[numPlayers - 1].GetComponentsInChildren<MapSelectViewport>();
        playerLayouts[numPlayers - 1].SetActive(true);
        allReadyImage.SetActive(false);
        for (int i = 0; i < viewports.Length; i++)
        {
            CheckController(playerInputs[_data.playerInfo[i].controllerID - 1]);
        }
    }

    private void Update()
    {
        if(_readyToStart && !_isLoading)
        {
            _isLoading = true;
            if (mineMapVotes > stellarMapVotes)
            {
                SceneManager.LoadSceneAsync(mineMapScene);
            }
            else if (stellarMapVotes > mineMapVotes)
            {
                SceneManager.LoadSceneAsync(stellarMapScene);
            }
            else
            {
                SceneManager.LoadSceneAsync(Random.Range(3,5));
            }
        }
    }

    private void CheckController(VehicleInput givenController)
    {
        bool inUse = false;
        for (int i = 0; i < viewports.Length; i++)
        {
            if (viewports[i].GetOccupied() && viewports[i].GetInput() == givenController)
            {
                Debug.Log("ishapp");
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
            if (!viewports[i].GetOccupied())
            {
                viewports[i].PlayerJoin(true, givenController);
                break;
            }
        }
    }

    public void UpdateData(bool isReady, int givenMapSelection)
    {
        if (isReady)
        {
            if (givenMapSelection == 1)
            {
                mineMapVotes = mineMapVotes + 1;
            }
            else if (givenMapSelection == 2)
            {
                stellarMapVotes = stellarMapVotes + 1;
            }
        }
        else if (!isReady)
        {
            if (givenMapSelection == 1)
            {
                mineMapVotes = mineMapVotes - 1;
            }
            else if (givenMapSelection == 2)
            {
                stellarMapVotes = stellarMapVotes - 1;
            }            
        }
        else
        {
            Debug.LogError("Invalid player map selection given");
        }
        _mineMapVoteText.text = mineMapVotes.ToString();
        _stellarMapVoteText.text = stellarMapVotes.ToString();
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
            if (viewports[i].GetOccupied() == true)
            {
                enoughPlayers = true;
                if (!viewports[i].GetSelected())
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

    public void LoadVehicleSelect()
    {
        SceneManager.LoadSceneAsync(vehicleSelectScene);
    }
}
