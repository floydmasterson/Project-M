using Photon.Pun;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootContainerControl : ItemContainer
{
    public enum ContainerType
    {
        Chest,
        Dropbag,
    }
    public enum ContainerTier
    {
        T0,
        T1,
        T2,
        T3,
    }
    [EnumToggleButtons, TabGroup("Setup")]
    public ContainerType containerType;
    [EnumToggleButtons, TabGroup("Setup")]
    public ContainerTier containerTier;
    [TabGroup("Setup")]
    [SerializeField] Transform itemsParent;
    [TabGroup("Setup")]
    [SerializeField] Inventory inventory;
    [TabGroup("Setup"), HideIf("@containerType != ContainerType.Dropbag")]
    [SerializeField] Item Gold;
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
    Character ECharacter;
    public IEnumerator Despawn(int time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (lootContainerManager != null)
            lootContainerManager.RemoveFromList(this);
        if (isActiveAndEnabled)
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
        if (containerType == ContainerType.Dropbag)
            StartCoroutine(Despawn(60));
        if (GameManger.Instance != null && containerTier == ContainerTier.T1)
            Destroy(gameObject, GameManger.Instance.gameTime);
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
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            StartCoroutine(Despawn(120));
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
        StopAllCoroutines();
        if (containerType == ContainerType.Chest && meshFilter != null)
            meshFilter.sharedMesh = open;
        isOpen = true;
        if (containerType == ContainerType.Chest)
            ChestOpen.PlaySFX();
        if (containerType == ContainerType.Dropbag)
            bagOpen.PlaySFX();
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        character.OpenItemContainer(this);
        ECharacter = character;
    }
    public void Close(Character character)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        if (isOpen)
            character.CloseItemContainer(this);
        if (containerType == ContainerType.Chest && meshFilter != null)
            meshFilter.sharedMesh = close;
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
                Item item = lootTable.GetRandom();
                inventory.AddItem(item, 0);
            }
            if (containerType == ContainerType.Dropbag)
            {
                inventory.AddItem(Gold, amount: goldDrop());
            }
        }
        if (randomAmount == true && BetterRandomAmount == false)
        {
            float Ramount = Random.Range(1, 3);
            for (int i = 0; i < Ramount; i++)
            {
                Item item = lootTable.GetRandom();
                inventory.AddItem(item, 0);
            }
        }
        if (BetterRandomAmount == true && randomAmount == true)
        {
            float Ramount = Random.Range(2, 4);
            for (int i = 0; i < Ramount; i++)
            {
                Item item = lootTable.GetRandom();
                inventory.AddItem(item, 0);
            }
        }
    }
    private int goldDrop()
    {
        int gold = 0;
        switch (containerTier)
        {
            case ContainerTier.T1:
                gold = Random.Range(15, 30);
                break;
            case ContainerTier.T2:
                gold = Random.Range(50, 80);
                break;
            case ContainerTier.T3:
                gold = Random.Range(90, 150);
                break;
            default: break;
        }
        return gold;
    }
    void CheckEmpty()
    {
        bool empty = true;
        foreach (ItemSlot slots in inventory.ItemSlots)
        {
            if (slots.Item != null)
            {
                empty = false;
                if (gameObject.activeInHierarchy)
                    StartCoroutine(Despawn(20));
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

