
using System.Text;
using UnityEngine;
[CreateAssetMenu(menuName = "Passives/Rage Max Passive")]
public class RageMaxPassive : PassiveSO
{
    MeeleController meleeController;

    public override void Passive()
    {
        meleeController.MaxRage += 1;
    }
    public override void ApplyPassive()
    {
        meleeController = PlayerUi.Instance.target.GetComponent<MeeleController>();
        if (meleeController != null)
            Passive();
    }
    public override void RemovePassive()
    {
        if (meleeController != null)
            meleeController.MaxRage -= 1;
    }
    public override string GetDescription()
    {
        return "Boundless Rage: This item fills you with rage increasing max rage by 1";


    }
}



