using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOver;
    //public GameObject countDown;

    private VehicleInput[] arr;


    private void Start()
    {
        arr = FindObjectsOfType<VehicleInput>();
        //Debug.Log(arr);        
        StartCoroutine(countDown(3));
    }

    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            if (pauseMenu.activeSelf == true)
            {
                turnOff(true);
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
            }
            else
            {
                turnOff(false);
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                
            }
        }
    }

    public void restart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void turnOff(bool stat)
    {
        foreach (VehicleInput t in arr)
        {
            t.setStatus(stat);

        }
    }

    IEnumerator countDown(int seconds)
    {
        turnOff(false);
        int count = seconds;

        while (count > 0)
        {
            Debug.Log(count);
            // display something...
            yield return new WaitForSeconds(1);
            count--;
        }

        // count down is finished...
        //StartGame();
        turnOff(true);  
    }
}
