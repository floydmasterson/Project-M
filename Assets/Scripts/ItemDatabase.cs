using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class ItemDatabase : MonoBehaviour
{
#if UNITY_EDITOR
    public static ItemDatabase instance;
    public SerializableDictionary<int, Item> items = new SerializableDictionary<int, Item>();
    private static int idCounter = 1;
    private void OnValidate()
    {
        if (idCounter <= 1)
            UpdateItems();
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdateItems()
    {
        idCounter = 1;
        items.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Item", new[] { "Assets/Prefabs/items/" });
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            Item item = (Item)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Item));
            items.Add(idCounter, item);
            item.itemId = idCounter++;
        }
    }
    public void Reset()
    {
        UpdateItems();
    }
    [Button]
    private void TableToFile()
    {
        using (StreamWriter file = new StreamWriter("Assets/Photon/PhotonUnityNetworking/Resources/ItemDataBase.txt"))
        {
            foreach (KeyValuePair<int, Item> pair in items)
            {
                file.WriteLine(pair.Key + "- " + pair.Value.ItemName);
            }
        }
    }
#endif
}
