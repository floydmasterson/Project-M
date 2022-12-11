using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class ChestControl : ItemContainer
{
    [TabGroup("Setup")]
    [SerializeField] Transform itemsParent;
    [TabGroup("Setup")]
    [SerializeField] Inventory inventory;
    [TabGroup("GFX")]
    [SerializeField] private MeshFilter meshFilter;
    [TabGroup("GFX")]
    [SerializeField] private Mesh close;
    [TabGroup("GFX")]
    [SerializeField] private Mesh open;
    [TabGroup("GFX")]
    public bool pickUpAllowed;
    public bool isOpen = false;
    Character character;
    [TabGroup("Loot Generation")]
    [SerializeField][Range(0, 17)] private int amount = 0;
    [TabGroup("Loot Generation")]
    [Tooltip("2-4 Drops")]
    [SerializeField] bool randomAmount = false;
    [TabGroup("Loot Generation")]
    [Tooltip(" 3-5 *Must enable both for better drops")]
    [SerializeField] bool BetterRandomAmount = false;
    [TabGroup("Loot Generation")]
    [TableList(AlwaysExpanded = true), HideLabel]
    public WeightedRandomList<Item> lootTable;

    public IEnumerator Despawn()
    {
        yield return new WaitForSecondsRealtime(20);
        Destroy(gameObject);
    }
    protected override void OnValidate()
    {
        if (itemsParent != null)
            itemsParent.GetComponentsInChildren(includeInactive: true, result: ItemSlots);
    }
    private void Awake()
    {
        LoadItems();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (meshFilter.sharedMesh != null)
                meshFilter.sharedMesh = open;
            pickUpAllowed = true;
            character = Character.Instance;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pickUpAllowed = true;
            character = Character.Instance;
        }
        if (isOpen == false && other.gameObject.CompareTag("Player"))
        {
            if (meshFilter.sharedMesh != null)
                meshFilter.sharedMesh = open;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (meshFilter.sharedMesh != null)
                meshFilter.sharedMesh = close;
            pickUpAllowed = false;
        }
    }
    public void Open()
    {
        isOpen = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        PlayerUi.Instance.Minimap.SetActive(false);
        character.OpenItemContainer(this);
        StopCoroutine(Despawn());
    }
    public void Close()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        PlayerUi.Instance.Minimap.SetActive(true);
        character.CloseItemContainer(this);
        if (meshFilter.sharedMesh != null)
            meshFilter.sharedMesh = close;
        StartCoroutine(Despawn());
        character = null;
        isOpen = false;
    }
    public void LoadItems()
    {
        if (amount > 0 && randomAmount == false && BetterRandomAmount == false)
        {
            for (int i = 0; i < amount; i++)
            {
                inventory.startingItems[i] = lootTable.GetRandom();
            }
        }
        if (randomAmount == true && BetterRandomAmount == false)
        {
            float Ramount = Random.Range(2, 4);
            for (int i = 0; i < Ramount; i++)
            {
                inventory.startingItems[i] = lootTable.GetRandom();
            }
        }
        if (BetterRandomAmount == true && randomAmount == true)
        {
            float Ramount = Random.Range(3, 5);
            for (int i = 0; i < Ramount; i++)
            {
                inventory.startingItems[i] = lootTable.GetRandom();
            }
        }
    }
}

