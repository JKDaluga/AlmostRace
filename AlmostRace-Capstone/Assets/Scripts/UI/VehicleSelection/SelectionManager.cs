using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    public int amountOfSelections = 2;
    public int nextSceneIndex = 2;
    public int mainMenuIndex = 0;
    public GameObject loadingText;
    public TextMeshProUGUI continueText;
    public TextMeshProUGUI backText;
    public Slider loadingBar;
    public ViewportController[] viewports;
    public VehicleInput[] playerInputs;
    public GameObject allReadyImage;
    private DataManager _data;
    private bool _readyToStart = false;
    private bool _isLoading = false;
    private int secondsElapsedLoading = 0;
    [HideInInspector] public bool controllersBeingUsed;


    // Start is called before the first frame update
    void Start()
    {
        _data = DataManager.instance;
        _isLoading = false;
        _data.ResetData();
        allReadyImage.SetActive(false);
        continueText.enabled = true;
        loadingText.SetActive(false);
    }

    private void Update()
    {
        if (!_isLoading)
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
                if(Input.GetButtonDown(playerInputs[i].selectButton))
                {
                    CheckController(playerInputs[i], "SelectButtonTriggered");
                }
            }
            
            if (_readyToStart)
            {
                if (controllersBeingUsed)
                {
                    continueText.text = "Press A To Continue";
                    backText.text = "Press B To<br>Go Back";
                }
                else
                {
                    continueText.text = "Press ENTER To Continue";
                    backText.text = "Press K To<br>Go Back";
                }
            }

            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                string controller = Input.GetJoystickNames()[i].ToString();
                if (controller != "")
                {
                    controllersBeingUsed = true;
                    break;
                }
                else
                {
                    controllersBeingUsed = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !_readyToStart)
            {
                LoadMainMenu();
            }

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

        if (!inUse && !_readyToStart && givenCommand == "AwakeButtonTriggered")
        {  
            AssignPlayer(givenController);
        }
        else if (givenCommand == "SelectButtonTriggered")
        {
            if (inUse && !_readyToStart && !selectedViewport.GetReady())
            {
                selectedViewport.VehicleSelect(true);
            }
            else if (inUse && _readyToStart && !_isLoading && selectedViewport.GetReady())
            {
                _isLoading = true;
                AudioManager.instance.Play("Menu Selection", this.transform);
                continueText.enabled = false;
                backText.enabled = false;
                loadingText.SetActive(true);
                StartCoroutine(LoadingMapScene(nextSceneIndex));
            }
        }
        else if (givenCommand == "BackButtonTriggered")
        {
            if (inUse && !selectedViewport.GetReady())
            {
                selectedViewport.PlayerJoin(false, null);
                AudioManager.instance.PlayWithoutSpatial("Menu Selection");
            }
            else if (inUse && selectedViewport.GetReady())
            {
                selectedViewport.VehicleSelect(false);
                AudioManager.instance.PlayWithoutSpatial("Menu Selection");
            }
            else if (!inUse && !_readyToStart)
            {
                LoadMainMenu();
            }
        }
    }

    private void LoadMainMenu()
    {
        _isLoading = true;
        SceneManager.LoadSceneAsync(mainMenuIndex);
        AudioManager.instance.PlayWithoutSpatial("Menu Selection");        
    }

    private void AssignPlayer(VehicleInput givenController)
    {
        for (int i = 0; i < viewports.Length; i++)
        {
            if (!viewports[i].GetJoined())
            {
                viewports[i].PlayerJoin(true, givenController);
                AudioManager.instance.PlayWithoutSpatial("Menu Selection");
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
        else
        {
            _readyToStart = false;
            allReadyImage.SetActive(false);
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

    private IEnumerator LoadingMapScene(int nextSceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneIndex);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBar.value = progress;

            yield return null;
        }
    }

    public bool GetReadyToStart()
    {
        return _readyToStart;
    }
    
}
