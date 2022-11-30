using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ChestControl : ItemContainer
{
    [Header("Needed Comp")]
    [SerializeField] Transform itemsParent;
    [SerializeField] Inventory inventory;
    [Header("OPen Close GFX")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Mesh close;
    [SerializeField] private Mesh open;
    public bool pickUpAllowed;
    public bool isOpen = false;
    Character character;
    [Header("Random Loot")]
    [SerializeField][Range(0, 17)] private int amount = 0;
    [Tooltip("3-6 Drops")]
    [SerializeField] bool randomAmount;
    [Tooltip(" 4-8 *Must enable both for better drops")]
    [SerializeField] bool BetterRandomAmount;
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
            meshFilter.mesh = open;
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
            StopAllCoroutines();
        }
        if (isOpen == false && other.gameObject.CompareTag("Player"))
        {
            meshFilter.mesh = open;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            meshFilter.mesh = close;
            pickUpAllowed = false;
            StartCoroutine(Despawn());
        }
    }
    public void Open()
    {
        isOpen = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        character.OpenItemContainer(this);
        StopCoroutine(Despawn());
    }
    public void Close()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        character.CloseItemContainer(this);
        meshFilter.mesh = close;
        StartCoroutine(Despawn());
        character = null;
        isOpen = false;
    }
    void LoadItems()
    {
        if (!randomAmount)
            for (int i = 0; i <= amount - 1; i++)
            {
                inventory.startingItems[i] = lootTable.GetRandom();

            }
        else if (randomAmount && !BetterRandomAmount)
        {
            Debug.Log("random");
            float Ramount = Random.Range(3, 6);
            for (int i = 0; i <= Ramount - 1; i++)
            {
                inventory.startingItems[i] = lootTable.GetRandom();

            }
        }
        else if (BetterRandomAmount && randomAmount)
        {
            Debug.Log("Better random");
            float Ramount = Random.Range(4, 8);
            for (int i = 0; i <= Ramount - 1; i++)
            {
                inventory.startingItems[i] = lootTable.GetRandom();

            }
        }

    }
}

