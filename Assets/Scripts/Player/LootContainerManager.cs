using Photon.Pun;
using System.Collections.Generic;

public class LootContainerManager : MonoBehaviourPun
{
    public List<LootContainerControl> ContinersInRange = new List<LootContainerControl>();

    public void AddToList(LootContainerControl continer)
    {
        if (!ContinersInRange.Contains(continer))
            ContinersInRange.Add(continer);
    }
    public void RemoveFromList(LootContainerControl continer)
    {
        if (ContinersInRange.Contains(continer))
            ContinersInRange.Remove(continer);
    }
    public void OpenContiner(Character character)
    {
        ContinersInRange[0].Open(character);
        ContinersInRange[0].Highlight(false);
    }
    public void CloseContiner(Character character)
    {
        ContinersInRange[0].Close(character);
        
    }
    public bool CanBeOpened()
    {
        LootContainerControl continer = ContinersInRange[0];
        if (continer != null && !continer.isOpen && continer.pickUpAllowed)
        {
            return true;
        }
        return false;
    }
    public bool CanBeClosed()
    {
        LootContainerControl continer = ContinersInRange[0];
        if (continer != null && continer.isOpen)
        {
            return true;
        }
        return false;
    }
    private void Update()
    {
        if (ContinersInRange.Count > 0 && photonView.IsMine && !ContinersInRange[0].isOpen)
        {
            ContinersInRange[0].Highlight(true);
            for (int i = 1; i < ContinersInRange.Count; i++)
            {
                ContinersInRange[i].Highlight(false);
            }
        }
    }
}
