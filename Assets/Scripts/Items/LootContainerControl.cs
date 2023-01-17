using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class LootContainerControl : ItemContainer
{
    public enum ContainerType
    {
        Chest,
        Dropbag,
    }
    [EnumToggleButtons]
    public ContainerType containerType;
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
    [TabGroup("GFX"), SerializeField]
    Outline outline;
    public bool pickUpAllowed;
    public bool playerInRange;
    public bool isOpen = false;
    [TabGroup("Loot Generation"), HideIf("@randomAmount || BetterRandomAmount")]
    [SerializeField][Range(0, 17)] private int amount = 0;
    [TabGroup("Loot Generation"), Tooltip("1-3 Drops"), HideIf("@amount > 0"), SerializeField]
    bool randomAmount = false;
    [TabGroup("Loot Generation"), ShowIf("@randomAmount"), Tooltip("2-4 *Must enable both for better drops"), SerializeField]
    bool BetterRandomAmount = false;
    [TabGroup("Loot Generation")]
    [TableList(AlwaysExpanded = true), HideLabel]
    public WeightedRandomList<Item> lootTable;
    [TabGroup("Audio"), SerializeField]
    SFX ChestOpen;
    [TabGroup("Audio"), SerializeField]
    SFX bagOpen;
    LootContainerManager lootContainerManager;
    bool popUpOpen;
    public IEnumerator Despawn()
    {
        yield return new WaitForSecondsRealtime(20);
        lootContainerManager.RemoveFromList(this);
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
        if (other.gameObject.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {      
            lootContainerManager = other.GetComponent<LootContainerManager>();
            lootContainerManager.AddToList(this);
            pickUpAllowed = true;
            playerInRange = true;
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (containerType == ContainerType.Chest && meshFilter.sharedMesh != null)
                meshFilter.sharedMesh = close;
            if (lootContainerManager != null)
                lootContainerManager.RemoveFromList(this);
            Highlight(false);
            pickUpAllowed = false;
            playerInRange = false;
            popUpOpen = false;
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }

    }
    public void Highlight(bool state)
    {
        if (state)
            outline.enabled = true;
        else if (!state)
            outline.enabled = false;
    }
    public void Open(Character character)
    {
        if (containerType == ContainerType.Chest && meshFilter.sharedMesh != null)
            meshFilter.sharedMesh = open;
        isOpen = true;
        if (containerType == ContainerType.Chest)
            ChestOpen.PlaySFX();
        if (containerType == ContainerType.Dropbag)
            bagOpen.PlaySFX();
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        character.OpenItemContainer(this);
        StopCoroutine("Despawn");
    }
    public void Close(Character character)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        character.CloseItemContainer(this);
        if (containerType == ContainerType.Chest && meshFilter.sharedMesh != null)
            meshFilter.sharedMesh = close;
        if (character != null)
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        CheckEmpty();
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
            float Ramount = Random.Range(1, 3);
            for (int i = 0; i < Ramount; i++)
            {
                inventory.startingItems[i] = lootTable.GetRandom();
            }
        }
        if (BetterRandomAmount == true && randomAmount == true)
        {
            float Ramount = Random.Range(2, 4);
            for (int i = 0; i < Ramount; i++)
            {
                inventory.startingItems[i] = lootTable.GetRandom();
            }
        }
    }
    void CheckEmpty()
    {
        bool empty = true;
        foreach (ItemSlot slots in inventory.ItemSlots)
        {
            if (slots.Item != null)
            {
                empty = false;
                StartCoroutine("Despawn");
                break;
            }
        }
        if (empty)
        {
            lootContainerManager.RemoveFromList(this);
            Destroy(gameObject);
        }
    }
 
}

