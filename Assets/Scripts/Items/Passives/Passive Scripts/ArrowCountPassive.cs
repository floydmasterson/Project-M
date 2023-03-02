using System.Text;
using UnityEngine;
[CreateAssetMenu(menuName = "Passives/Arrow Count Passive")]
public class ArrowCountPassive : PassiveSO
{
    public int quiverAmount;
    public int PassiveLvl;
    public string QuiverName;
    ArrowController AC;
    StringBuilder sb = new StringBuilder();
    public override void ApplyPassive()
    {
        AC = PlayerUi.Instance.target.GetComponent<ArrowController>();
        if (AC != null)
            Passive();
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        sb.Append(QuiverName + ": Increase max arrow amount by " + quiverAmount + ".");
        return sb.ToString();

    }

    public override void Passive()
    {
        if (AC != null)
            AC.MaxArrows += quiverAmount;
    }

    public override void RemovePassive()
    {
        if (AC != null)
            AC.MaxArrows -= quiverAmount;
    }
}
