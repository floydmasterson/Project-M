using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] Text ItemNameText;
    [SerializeField] Text ItemTypeText;
    [SerializeField] Text ItemDescriptionText;
    [SerializeField] bool sell = true;

    private void OnEnable()
    {
        PlayerManger.onInventoryClose += HideTooltip;
    }
    private void OnDisable()
    {
        PlayerManger.onInventoryClose -= HideTooltip;
    }
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowTooltip(Item item)
    {
        ItemNameText.text = item.ItemName;
        if (item.ItemTier == Item.Tier.T0)
        {
            ItemTypeText.text = "<color=#CABFAA>" + item.GetTier() + " </color>" + item.GetItemType().Replace("_", " ");
        }
        if (item.ItemTier == Item.Tier.T1)
        {
            ItemTypeText.text = "<color=#B2C895>" + item.GetTier() + " </color>" + item.GetItemType().Replace("_", " ");
        }
        if (item.ItemTier == Item.Tier.T2)
        {
            ItemTypeText.text = "<color=#9A4355>" + item.GetTier() + " </color>" + item.GetItemType().Replace("_", " ");
        }
        if (item.ItemTier == Item.Tier.T3)
        {
            ItemTypeText.text = "<color=#532B47>" + item.GetTier() + " </color>" + item.GetItemType().Replace("_", " ");
        }
        ItemDescriptionText.text = item.GetDescription(sell);
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
