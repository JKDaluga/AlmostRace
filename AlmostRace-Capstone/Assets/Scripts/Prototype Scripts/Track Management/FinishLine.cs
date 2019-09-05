using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    private TrackManager trackManager;
    public Text WinText;
    private bool gameDone;
    // Start is called before the first frame update
    void Start()
    {
        trackManager = GameObject.Find("Manager").GetComponent<TrackManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            if (trackManager.CheckIncrimate() == true)
            {
                trackManager.AssignIncrimate(false);
                if(trackManager.lap < 3)
                {
                   trackManager.lap++; 
                }
                else if (trackManager.lap >= 3)
                {
                    if (gameDone == false)
                        StartCoroutine(WinCycle(other.gameObject));
                }
            }
        }
    }

    IEnumerator WinCycle(GameObject other)
    {
        gameDone = true;
        if (other.gameObject.name == "Player1(8_22_2019)")
        {
            WinText.text = "Player 1 Wins";
        }
        else if (other.gameObject.name == "Player2(8_22_2019) (1)")
        {
            WinText.text = "Player 2 Wins";
        }
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("MainMenu");
    }
}
