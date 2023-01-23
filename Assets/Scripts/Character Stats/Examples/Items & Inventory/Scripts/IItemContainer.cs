
public interface IItemContainer
{
    bool CanAddItem(Item item, int amount = 1);
    bool AddItem(Item item, int i, int amount);

    Item RemoveItem(string itemID);
    bool RemoveItem(Item item, int amount);
    void Clear();

    int ItemCount(string itemID);
}
