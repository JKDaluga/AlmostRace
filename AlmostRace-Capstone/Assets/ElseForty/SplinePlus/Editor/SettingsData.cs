using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ElseFortySettings/Data", order = 1)]
public class SettingsData : ScriptableObject
{
     public bool ShowRaycast = true;
     public bool ShowGizmos = true;
    
     public bool ShowSecondaryHandles = true;
     public bool ShowHelper = false;
     public NodeType NodeType = NodeType.Smooth;
    
    
     public float HelperSize = 1;
     public float GizmosSize = 0.2f;
    
     public Color StandardPathPointColor;
     public Color RandomSharedNodeColor;
     public Color DefinedSharedNodeColor;

    private void OnEnable()
    {
         StandardPathPointColor = Color.green;
         RandomSharedNodeColor = Color.magenta;
         DefinedSharedNodeColor = Color.blue;
    }
}
