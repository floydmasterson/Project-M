using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ChestControl : ItemContainer
{

    public bool pickUpAllowed;
    public bool isOpen = false;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Mesh close;
    [SerializeField] private Mesh open;
    [Space]
    [SerializeField] Transform itemsParent;
    [Space]
    Character character;
    [SerializeField] Inventory inventory;

    [SerializeField][Range(0, 17)] private int amount = 0;
    [Tooltip("1-5 Drops")]
    [SerializeField] bool randomAmount;
    [Tooltip(" 3-5 *Must enable both for better drops")]
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
        checkEmpty();
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
            float Ramount = Random.Range(1, 5);
            for (int i = 0; i <= Ramount - 1; i++)
            {
                inventory.startingItems[i] = lootTable.GetRandom();

            }
        }
        else if (BetterRandomAmount && randomAmount)
        {
            Debug.Log("Better random");
            float Ramount = Random.Range(3, 5);
            for (int i = 0; i <= Ramount - 1; i++)
            {
                inventory.startingItems[i] = lootTable.GetRandom();

            }
        }

    }



    private void checkEmpty()
    {

        int empty = 0;
        int full = 0;
        for (int i = 0; i < inventory.startingItems.Length; i++)
        {
            if (inventory.startingItems[i] == null)
            {
                empty++;
            }
            else
            {
                full++;
            }
        }
        if (full > 0)
        {
            Debug.Log("full");
            return;
        }
        else
        {

            Destroy(gameObject);
            //partical  e   
        }
    }
}

