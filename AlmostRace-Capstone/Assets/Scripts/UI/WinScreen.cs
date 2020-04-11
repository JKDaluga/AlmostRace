using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{

    DataManager players;
    VehicleInput playerNum;
    HypeManager playerHype;

    string[] awardList = {"Spray n' Pray", "Speed Demon", "Shields Up", "Slippery"};

    int[] awardWinners;
    public float smallHypeAward,mediumHypeAward,largeHypeAward;

    int winningPlayer;

    [Header("Scrolling Text Values")]
    public float scrollPause = 2.5f;
    public float scrollSpeed = .0025f;

    // Start is called before the first frame update
    void Start()
    {
        players = FindObjectOfType<DataManager>();
        playerNum = FindObjectOfType<VehicleInput>();
        awardWinners = new int[awardList.Length];
        winningPlayer = 0;
        playerHype = GetComponent<HypeManager>();
    }

    public void chooseWinners()
    {
        for (int i = 0; i < awardList.Length; i++)
        {
            maxValueWinner(awardList[i], i);
        }
    }

    private void maxValueWinner(string awardName, int award)
    {
        switch (awardName)
        {
            case "Spray n' Pray":
                awardWinners[award] = offensiveAbilityMax();
                break;
            case "Speed Demon":
                awardWinners[award] = boostAbilityMax();
                break;
            case "Shields Up":
                awardWinners[award] = defensiveAbilityMax();
                break;
            case "Slippery":
                awardWinners[award] = driftAbilityMax();
                break;
        }
        
    }

    private int offensiveAbilityMax()
    {
        for (int i = 0; i<players.playerInfo.Length; i++)
        {
            if(players.playerInfo[i].offensiveAbilityUsed>players.playerInfo[winningPlayer].offensiveAbilityUsed)
            {
                winningPlayer = i;
            }
        }
        addHype(winningPlayer + 1, smallHypeAward, "Spray n' Pray");
        return winningPlayer;
    }

    private int defensiveAbilityMax()
    {
        for (int i = 0; i < players.playerInfo.Length; i++)
        {
            if (players.playerInfo[i].defenseAbilityUsed > players.playerInfo[winningPlayer].defenseAbilityUsed)
            {
                winningPlayer = i; 
            }
        }
        addHype(winningPlayer + 1, smallHypeAward, "Shields Up");
        return winningPlayer;
    }

    private int boostAbilityMax()
    {
        for (int i = 0; i < players.playerInfo.Length; i++)
        {
            if (players.playerInfo[i].boostAbilityUsed > players.playerInfo[winningPlayer].boostAbilityUsed)
            {
                winningPlayer = i;
            }
        }
        addHype(winningPlayer + 1, smallHypeAward, "Speed Demon");
        return winningPlayer;
    }

    private int driftAbilityMax()
    {
        for (int i = 0; i < players.playerInfo.Length; i++)
        {
            if (players.playerInfo[i].driftTimer > players.playerInfo[winningPlayer].driftTimer)
            {
                winningPlayer = i;
            }
        }
        addHype(winningPlayer + 1, smallHypeAward, "Slippery");
        return winningPlayer;
    }

    private void addHype(int playerNum, float amount, string awardTitle)
    {
        DataManager.instance.playerInfo[playerNum - 1].hypeAmount += amount;
        playerHype.awards[playerNum - 1] += awardTitle + "\n";
        playerHype.awardsNumbers[playerNum - 1] += amount + "\n";
    }
}
