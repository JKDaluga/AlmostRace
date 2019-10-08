using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SplinePlusAnimation))]
public class SplinePlusAnimationEditor : Editor
{
    SPData SPData;

    void OnEnable()
    {
        var SplinePlusAnimation = (SplinePlusAnimation)target;

        SPData = SplinePlusAnimation.gameObject.GetComponent<SplinePlus>().SPData;
    }

    private void OnSceneGUI()
    {
        var Animator = SPData.DataParent.GetComponent<Animator>();
        if (Animator != null && SceneView.focusedWindow!=null && SceneView.focusedWindow.titleContent.text == "Animation")
        {
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
        }
    }
}
