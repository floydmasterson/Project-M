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
    [TabGroup("GFX")]
    public bool pickUpAllowed;
    public bool playerInRange;
    public bool isOpen = false;
    Character character;
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
        if (other.gameObject.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
            playerInRange = true;
            pickUpAllowed = true;
            character = Character.Instance;
        }
        else if(other.gameObject.CompareTag("Chest"))
        {
            pickUpAllowed = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Chest"))
        {
            if(playerInRange)
                pickUpAllowed = true;
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            if (containerType == ContainerType.Chest && meshFilter.sharedMesh != null)
                meshFilter.sharedMesh = close;
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            pickUpAllowed = false;
            character = null;
            playerInRange = false;
        }
        
    }
    public void Open()
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
        PlayerUi.Instance.Minimap.SetActive(false);
        character.OpenItemContainer(this);
        StopCoroutine("Despawn");
    }
    public void Close()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        PlayerUi.Instance.Minimap.SetActive(true);
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
            Destroy(gameObject);
    }

}

