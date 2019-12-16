using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    Author: Jason Daluga
    Purpose: used for asyncronous loading of any scene
    CURRENTLY UNUSED
*/

public class SceneLoader : MonoBehaviour
{
    GameObject loadingScreen;

    IEnumerator LoadNewScene(int scene)
    {
        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        yield return new WaitForSeconds(3);

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        loadingScreen.SetActive(true);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            yield return null;
        }

        loadingScreen.SetActive(false);
    }
}
