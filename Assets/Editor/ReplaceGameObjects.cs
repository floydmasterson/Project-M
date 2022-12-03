using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Member;
// CopyComponents - by Michael L. Croswell for Colorado Game Coders, LLC
// March 2010

public class ReplaceGameObjects : EditorWindow
{
    public bool copyValues = true;
    public GameObject NewType;

    [MenuItem("Window/Replace GameObjects")]
    static void ShowWindow()
    {
        GetWindow<ReplaceGameObjects>("Replace Game Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("Replace old objects with Selected Prefab");
        NewType = EditorGUILayout.ObjectField("Prefab", NewType, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("Replace"))
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                GameObject newObject;
                newObject = (GameObject)PrefabUtility.InstantiatePrefab(NewType);
                if(copyValues == true)
                {
                newObject.transform.position = go.transform.position;
                newObject.transform.rotation = go.transform.rotation;
                newObject.transform.parent = go.transform.parent;
                }

                DestroyImmediate(go);

            }

        }

    }
}