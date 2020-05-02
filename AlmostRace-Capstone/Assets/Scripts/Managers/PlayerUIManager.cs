﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/*
    Author: Eddie Borrisov
    Purpose: Sets up correct colors for player UI
*/
public class PlayerUIManager : MonoBehaviour
{
    [Header("Script Variables")]
    [Space(30)]
    public VehicleInput vehicleInputScript;
    private HypeManager _hypeManager;
    private RaceManager _rm;
    private DataManager _dm;
    private CarHealthBehavior _chb;
    private List<Transform> _attacksInRange = new List<Transform>();
    public GameObject weaponsActivatedText;

    [Header("Kill Popup Variables")]
    [Space(30)]

    public RectTransform killPopup;
    public Image killPopupImage;
    public TextMeshProUGUI killPopupText;
    private int _killPopupAmount = 0;
    private bool _usingKillPopup = false;

    public RectTransform killPopupSpawn;
    public RectTransform killPopupTarget;

    [Header("Time Popup Variables")]
    [Space(30)]
    public RectTransform timePopup;
    public Image timePopupImage;
    public TextMeshProUGUI timePopupText;
    public RectTransform timePopupSpawn;
    public RectTransform timePopupTarget;
    private bool _usedTimePopup = false;


    [Header("HypeDisplay Variables")]
    [Space(30)]
    public List<Sprite> hypeDisplayColors;
    public Image hypeDisplay;
    public List<Vector3> hypeTextColors;
    public TextMeshProUGUI hypeText;


    [Header("Scoring Variables")]
    [Space(30)]
    public TextMeshProUGUI KillCount;
    public TextMeshProUGUI LapTimer;


    [Header("ArenaHype Variables")]
    [Space(30)]
    public GameObject arenaStatus;
    public Image lockImage;
    public Sprite lockSprite;
    public Sprite unlockSprite;
    public Image lockBottomFill;
    public Image lockTopFill;

    public TextMeshProUGUI arenaHypeText;


    public TextMeshProUGUI arenaHypeNumber;

    [Header("Pole Position Variables")]
    [Space(30)]
    public List<Sprite> beanSprites;
    public Image poleBean;

    [Header("Incoming Indicator Variables")]
    [Space(30)]
    public Camera localCam;
    public Transform thisVehicle;
    public float indicatorScaling = 1;
    public float toCloseDistance = 10;
    public Vector3 onePlayerScale = new Vector3(0.8f, 0.8f, 1.0f);
    public Vector3 multiplePlayerScale = new Vector3(1f, 1f, 1f);
    public List<GameObject> attackIndicators = new List<GameObject>();
    private Vector3 offSetVector = Vector3.zero;
    private float _heightOffset = 0.045f;

    private RaycastCar _raycastCarHolder;

    public void ResetWeaponText(float time)
    {
        Invoke("ResetWeaponTextActual", time);
    }

    private void ResetWeaponTextActual()
    {
        weaponsActivatedText.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        arenaStatus.SetActive(false);
        if (GameObject.FindGameObjectWithTag("GameManager") != null)
        {
            _hypeManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HypeManager>();
            _rm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<RaceManager>();
        }
        else
        {
            Debug.LogWarning("Game Manager Can Not be Found");
        }
        int numPlayers = 1;
        if (DataManager.instance != null)
        {
           numPlayers = DataManager.instance.getNumActivePlayers();
            _dm = DataManager.instance;
     
        }
        else
        {
            Debug.LogWarning("Data Manager Can Not be Found");
        }

        int playerNum = vehicleInputScript.getPlayerNum();
        _chb = vehicleInputScript.GetComponent<CarHealthBehavior>();

        _raycastCarHolder = vehicleInputScript.GetComponent<RaycastCar>();
        _dm.playerUIDictionary.Add(_raycastCarHolder.playerID, this);

        //Old Attack Indicator Setup
        /*
        if (numPlayers > 1)
        {
            _heightOffset = 0.057f;
            for (int i = 0; i < attackIndicators.Count; i++)
            {
                attackIndicators[i].transform.localScale = multiplePlayerScale;
            }
        }
        else
        {
            for (int i = 0; i < attackIndicators.Count; i++)
            {
                attackIndicators[i].transform.localScale = onePlayerScale;
            }
        }
        */
        switch (playerNum)
        {
            case 1: //is player 1
                if (numPlayers == 2)
                {
                    offSetVector = new Vector3(0, localCam.pixelHeight, 0);
                }
                else if(numPlayers >= 3)
                {
                    offSetVector = new Vector3(0, localCam.pixelHeight, 0);
                }
                break;

            case 2: //is player 2
                if(numPlayers >= 3)
                {
                    offSetVector = new Vector3(localCam.pixelWidth, localCam.pixelHeight, 0);
                }
                break;

            case 3: //is player 3
                offSetVector = new Vector3(0, 0, 0);
                break;

            case 4: //is player 4
                offSetVector = new Vector3(localCam.pixelWidth, 0, 0);
                break;

        }
        //Legacy Hype and Pole position Setters, Assigned per player
        //hypeText.color = Color.cyan;
        //hypeDisplay.sprite = hypeDisplayColors[playerNum - 1];
        //poleBean.sprite = beanSprites[playerNum - 1];
    }

    void Update()
    {
        KillCount.text = "KILLS\n" + _dm.playerInfo[_raycastCarHolder.playerID - 1].numKills;
        if (_raycastCarHolder.inArena)
        {
            LapTimer.text = DataManager.instance.convertTime(DataManager.instance.playerInfo[_raycastCarHolder.playerID-1].timerRace1);
        }
        else if (_raycastCarHolder.finished)
        {
            LapTimer.text = DataManager.instance.convertTime(DataManager.instance.playerInfo[_raycastCarHolder.playerID-1].timerRace2);
        }
        else
        {
            LapTimer.text = _rm.timeText;
        }

        if(_chb.healthCurrent <= 0)
        {
            _attacksInRange.Clear();
        }

        //Old Attack Indicator Call
        //UpdateAttackIndicators();

        //TestKillPopup();
        TestTriggerTimePopup();
    }

    // Keeps track of, updates the position, and shows attack indicators
    private void UpdateAttackIndicators()
    {
        TextMeshProUGUI currentText;
        GameObject currentExclamation;
        Vector3 targetPos, relativePos;
        float currentDistance, x;

        if (_attacksInRange.Count > 0)
        {
            for (int i = 0; i < _attacksInRange.Count; i++)
            {
                if (_attacksInRange[i] == null)
                {
                    _attacksInRange.RemoveAt(i);
                }
                else if (i < attackIndicators.Count)
                {
                    targetPos = _attacksInRange[i].transform.position;
                    relativePos = thisVehicle.InverseTransformPoint(targetPos);
                    currentDistance = Mathf.Round(Vector3.Distance(thisVehicle.transform.position, targetPos));

                    x = Mathf.Clamp((relativePos.x * (localCam.pixelWidth / indicatorScaling)) + localCam.pixelWidth / 2, localCam.pixelWidth * 0.02f, localCam.pixelWidth * 0.98f);

                    if (attackIndicators[i] != null)
                    {
                        if (attackIndicators[i].activeSelf == false)
                        {
                            attackIndicators[i].SetActive(true);
                        }
                        attackIndicators[i].transform.position = new Vector3(x, localCam.pixelHeight * _heightOffset, 0) + offSetVector;
                        currentText = attackIndicators[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                        currentExclamation = attackIndicators[i].transform.GetChild(2).gameObject;
                        if (currentDistance > toCloseDistance)
                        {
                            currentExclamation.SetActive(false);
                            currentText.color = new Color32(0, 0, 0, 255);
                            currentText.text = currentDistance.ToString() + "m";
                        }
                        else
                        {
                            currentText.text = "";
                            //currentText.color = new Color32(160, 0, 0, 255);
                            currentExclamation.SetActive(true);
                        }
                    }
                    else
                    {
                        attackIndicators[i].SetActive(false);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < attackIndicators.Count; i++)
            {
                if (attackIndicators[i].activeSelf == true)
                {
                    attackIndicators[i].SetActive(false);
                }
            }
        }
    }

    public void TriggerKillPopup()
    {
     
        StopAllCoroutines();
        killPopupText.color = new Color32(255, 255, 255, 255);
        killPopupImage.color = new Color32(255, 255, 255, 255);
        if (_usingKillPopup == false)
        {
            _usingKillPopup = true;
            killPopup.position = killPopupSpawn.position;
        }
        _killPopupAmount += 1;
        killPopupText.text = "+" + _killPopupAmount + "\nKills";



        StartCoroutine(MoveKillPopup());
    }



    public void TestKillPopup()
    {
     
        if (Input.GetKeyDown(KeyCode.T))
        {

            StopAllCoroutines();
            killPopupText.color = new Color32(255, 255, 255, 255);
            killPopupImage.color = new Color32(255, 255, 255, 255);
            if (_usingKillPopup == false)
            {
                _usingKillPopup = true;
                killPopup.position = killPopupSpawn.position;
            }
            _killPopupAmount += 1;
            killPopupText.text = "+" + _killPopupAmount + "\nKills";

         
            StartCoroutine(MoveKillPopup());
        }
    }

    public IEnumerator MoveKillPopup()
    {
      
        while (killPopup.position.y < killPopupTarget.position.y)
        {
            killPopup.position += new Vector3(0, 10f, 0);
            yield return new WaitForSeconds(.025f);
        }

        StartCoroutine(FadeKillPopup());
    }

    public IEnumerator FadeKillPopup()
    {
        while(killPopupImage.color.a > 0)
        {
            killPopupImage.color -= new Color32(0,0,0,5);
            killPopupText.color -= new Color32(0, 0, 0, 5);
            yield return new WaitForSeconds(.01f);
        }
        _usingKillPopup = false;
        _killPopupAmount = 0;
    }

    public void TriggerTimePopup()
    {
        if(!_usedTimePopup)
        {
            timePopup.position = timePopupSpawn.position;
            _usedTimePopup = true;
            timePopupText.text = LapTimer.text.ToString();
            timePopupText.color = new Color32(255, 255, 255, 255);
            timePopupImage.color = new Color32(0, 0, 0, 255);
            InvokeRepeating("MoveTimePopup", 0, .25f);
        }
  
    }

    public void TestTriggerTimePopup()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            timePopup.position = timePopupSpawn.position;
            timePopupText.text = LapTimer.text;
            timePopupText.color = new Color32(255, 255, 255, 255);
            timePopupImage.color = new Color32(0, 0, 0, 255);
            InvokeRepeating("MoveTimePopup", 0, .01f);
        }
    }

    public void MoveTimePopup()
    {
        if (timePopup.position.y >= timePopupTarget.position.y)
        {
            CancelInvoke("MoveTimePopup");
            InvokeRepeating("FadeTimePopup", 0, .01f);
            return;
        }
        timePopup.position += new Vector3(0, 2f, 0);
       
    }

    public void FadeTimePopup()
    {
        if (timePopupImage.color.a <= 0)
        {
            CancelInvoke("FadeTimePopup");
            return;
        }
        timePopupImage.color -= new Color32(0, 0, 0, 5);
        timePopupText.color -= new Color32(0, 0, 0, 5);
       
    }

    // Adds an attack to the list of incoming attacks
    public void AddToAttacksInRange(Transform givenAttackTransform)
    {
        _attacksInRange.Add(givenAttackTransform);
    }

    // Removes an attack after the attack is over
    public void RemoveAttackInRange(Transform givenAttackTransform)
    {
        for(int i = _attacksInRange.Count - 1; i >= 0; i--)
        {
            if(_attacksInRange[i] == givenAttackTransform)
            {
                _attacksInRange.RemoveAt(i);
            }
        }
    }

    public int AttacksInRangeCount()
    {
        return _attacksInRange.Count;
    }

    public void ActivateArenaHypeDisplay()
    {
        arenaStatus.SetActive(true);
    }

    public void DeactivateArenaHypeDisplay()
    {
        LockArena();
        arenaStatus.SetActive(false);
    }

    public void LockArena()
    {
        lockBottomFill.fillAmount = 0;
        lockTopFill.fillAmount = 0;
        lockImage.sprite = lockSprite;
    }

    public void UnlockArena()
    {
        lockTopFill.fillAmount = 0;
        lockImage.sprite = unlockSprite;
    }

    public void SetArenaHypeDisplayNumber(float minutes, float seconds)
    {
        arenaHypeNumber.text = string.Format("{0}:{1}", minutes.ToString("0"), seconds.ToString("00"));
    }
}
