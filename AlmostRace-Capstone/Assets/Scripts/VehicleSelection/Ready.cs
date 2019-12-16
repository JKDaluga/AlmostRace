using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    Creator and developer of script: Leonardo Caballero
    Purpose: Once the players are ready and have chosen their vehicles, it allows anyone to press start
    to load the map. 
*/

public class Ready : MonoBehaviour
{

    public string sceneName;

    // Update is called once per frame
    void fixedUpdate()
    {
        if (Input.GetButtonDown("Submit")){
            //SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
