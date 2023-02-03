using UnityEngine;

public class DataBaseCache : MonoBehaviour
{
    public SerializableDictionary<int, Item> items = new SerializableDictionary<int, Item>();
    [SerializeField] private ItemDatabase itemDatabase;
    public static DataBaseCache instance { get; private set; }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (items.Count <= 0)
            LoadData();
    }
    private void LoadData()
    {
        items = itemDatabase.items;
    }
#endif
    private void Awake()
    {
        instance = this;
    }
    public Item GetItem(int itemId)
    {
        Item itemToGet = items[itemId];
        return itemToGet.GetCopy();
    }
}
