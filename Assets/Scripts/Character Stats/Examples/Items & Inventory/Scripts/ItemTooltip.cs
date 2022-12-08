using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] Text ItemNameText;
    [SerializeField] Text ItemTypeText;
    [SerializeField] Text ItemDescriptionText;

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
        ItemTypeText.text = item.GetItemType().Replace("_", " ");
        ItemDescriptionText.text = item.GetDescription();
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
