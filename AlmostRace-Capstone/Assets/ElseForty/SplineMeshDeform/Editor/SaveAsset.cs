
using UnityEditor;
using UnityEngine;

public class SaveAsset  {

 
	public static void Save( GameObject Obj , string assetName) {

        if (!AssetDatabase.IsValidFolder("Assets/SMDExport/Meshes"))// create directory if not exist
        {
            System.IO.Directory.CreateDirectory("Assets/SMDExport/Meshes");
            AssetDatabase.Refresh();
        }
       
        if ( AssetDatabase.IsValidFolder("Assets/SMDExport/Meshes"))
        {
            var url ="Assets/SMDExport/Meshes/"+ assetName + ".asset";
            // create mesh asset
           if(!AssetDatabase.LoadAssetAtPath(url, typeof(Mesh)))
            {
 
                AssetDatabase.CreateAsset(Obj.GetComponent<MeshFilter>().sharedMesh, url);
                 AssetDatabase.SaveAssets();

                //load mesh  
                var mesh = (Mesh)AssetDatabase.LoadAssetAtPath(url, typeof(Mesh));


                // pass mesh  to clone game object mesh filter
                var instance = new GameObject();

                var cloneMesh = instance.AddComponent<MeshFilter>();
                instance.AddComponent<MeshRenderer>().materials = Obj.GetComponent<MeshRenderer>().sharedMaterials;
                cloneMesh.sharedMesh = mesh;

                // create clone prefab
                PrefabUtility.CreatePrefab( "Assets/SMDExport/" + assetName + ".prefab", instance);
                // destroy instance game object
                MonoBehaviour.DestroyImmediate(instance);

                EditorUtility.DisplayDialog(
                "Success",
                "Asset exported successfully , it can be found here 'Assets/SMDExport/' ",
                "Ok");

            }
            else
            {
                EditorUtility.DisplayDialog(
                    "error",
                    "Asset already exist, please change asset name!",
                    "Ok");
            }
        }
    }
}
