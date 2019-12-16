using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    Author: Eddie Borrisov
    Purpose: Sets up correct colors for player UI
    CURRENTLY UNUSED
*/
public class PlayerUIManager : MonoBehaviour
{
    [Header("Script Variables")]
    [Space(30)]
    public VehicleInput vehicleInputScript;
    private HypeManager _hypeManager;

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

    // Start is called before the first frame update
    void Start()
    {
        arenaHype.SetActive(false);
        _hypeManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HypeManager>();

        int playerNum = vehicleInputScript.getPlayerNum();
        switch(playerNum)
        {
            case 1: //is player 1
                hypeText.color = Color.cyan;
                hypeDisplay.sprite = hypeDisplayColors[playerNum - 1];
                poleBean.sprite = beanSprites[playerNum - 1];
                break;

            case 2: //is player 2
                hypeText.color = Color.yellow;
                hypeDisplay.sprite = hypeDisplayColors[playerNum - 1];
                poleBean.sprite = beanSprites[playerNum - 1];
                break;

            case 3: //is player 3
                hypeText.color = Color.green;
                hypeDisplay.sprite = hypeDisplayColors[playerNum - 1];
                poleBean.sprite = beanSprites[playerNum - 1];
                break;

            case 4: //is player 4
                hypeText.color = Color.red;
                hypeDisplay.sprite = hypeDisplayColors[playerNum - 1];
                poleBean.sprite = beanSprites[playerNum - 1];
                break;

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
