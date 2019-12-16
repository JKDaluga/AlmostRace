using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
