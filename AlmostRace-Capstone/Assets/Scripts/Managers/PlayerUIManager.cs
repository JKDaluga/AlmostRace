using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private List<Transform> _attacksInRange = new List<Transform>();

    [Header("HypeDisplay Variables")]
    [Space(30)]
    public List<Sprite> hypeDisplayColors;
    public Image hypeDisplay;
    public List<Vector3> hypeTextColors;
    public TextMeshProUGUI hypeText;


    [Header("ArenaHype Variables")]
    [Space(30)]
    public GameObject arenaHype;
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
    [Space(20)]
    public Camera localCam;
    public Transform thisVehicle;
    public float indicatorScaling = 1;
    public List<Image> attackIndicators = new List<Image>();
    private Vector3 offSetVector = Vector3.zero;
    private float _heightOffset = 0.06f;

    // Start is called before the first frame update
    void Start()
    {
        arenaHype.SetActive(false);
        _hypeManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HypeManager>();
        DataManager dataManager = DataManager.instance;
        int numPlayers = dataManager.getNumActivePlayers();

        int playerNum = vehicleInputScript.getPlayerNum();

        if (numPlayers > 1)
        {
            _heightOffset = 0.09f;
        }

        switch(playerNum)
        {
            case 1: //is player 1
                hypeText.color = Color.cyan;
                hypeDisplay.sprite = hypeDisplayColors[playerNum - 1];
                poleBean.sprite = beanSprites[playerNum - 1];
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
                hypeText.color = Color.yellow;
                hypeDisplay.sprite = hypeDisplayColors[playerNum - 1];
                poleBean.sprite = beanSprites[playerNum - 1];
                if(numPlayers >= 3)
                {
                    offSetVector = new Vector3(localCam.pixelWidth, localCam.pixelHeight, 0);
                }
                break;

            case 3: //is player 3
                hypeText.color = Color.green;
                hypeDisplay.sprite = hypeDisplayColors[playerNum - 1];
                poleBean.sprite = beanSprites[playerNum - 1];
                offSetVector = new Vector3(0, 0, 0);
                break;

            case 4: //is player 4
                hypeText.color = Color.red;
                hypeDisplay.sprite = hypeDisplayColors[playerNum - 1];
                poleBean.sprite = beanSprites[playerNum - 1];
                offSetVector = new Vector3(localCam.pixelWidth, 0, 0);
                break;

        }
    }

    void Update()
    {
        // Attack Indicators
        if (_attacksInRange.Count > 0)
        {
            for (int i = 0; i < _attacksInRange.Count; i++)
            {
                if (_attacksInRange[i] == null)
                {
                    _attacksInRange.RemoveAt(i);
                }
                else
                {
                    Vector3 targetPos = _attacksInRange[i].transform.position;
                    Vector3 relativePos = thisVehicle.InverseTransformPoint(targetPos);

                    float angle = Mathf.Atan2(relativePos.y, relativePos.x);
                    angle += 90 * Mathf.Deg2Rad;
                    float x = Mathf.Clamp((relativePos.x * (localCam.pixelWidth/ indicatorScaling)) + localCam.pixelWidth / 2, localCam.pixelWidth * 0.02f, localCam.pixelWidth * 0.98f);

                    if (attackIndicators[i] != null)
                    { 
                        if (attackIndicators[i].enabled == false)
                        {
                            attackIndicators[i].enabled = true;
                        }
                        attackIndicators[i].transform.position = new Vector3(x, localCam.pixelHeight * _heightOffset, 0) + offSetVector;
                        //attackIndicators[i].transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
                    }
                    else
                    {
                        attackIndicators[i].enabled = false;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < attackIndicators.Count; i++)
            {
                if (attackIndicators[i].enabled == true)
                {
                    attackIndicators[i].enabled = false;
                }
            }
        }
    }

    public void AddToAttacksInRange(Transform givenAttackTransform)
    {
        _attacksInRange.Add(givenAttackTransform);
    }

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

    public void ActivateArenaHypeDisplay()
    {
        arenaHype.SetActive(true);
    }

    public void DeactivateArenaHypeDisplay()
    {
        LockArena();
        arenaHype.SetActive(false);
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

    public void SetArenaHypeDisplayNumber(float hypePercentage)
    {
        arenaHypeNumber.text = "" + hypePercentage.ToString("F0") + "%";
    }
}
