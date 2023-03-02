using UnityEngine;

public static class DamageCaculator
{
    public enum Reciver
    {
        Mob,
        Player,
    }
    public static int MagicDamage(Character character, int reciver, Enemys mob, PlayerManger player, int additionalDmg = 0)
    {
        switch (reciver)
        {
            case 0:
                return additionalDmg + (Mathf.RoundToInt(character.Intelligence.Value / Mathf.Pow(3f, mob.Defense / character.Intelligence.Value)));
            case 1:
                return additionalDmg + (Mathf.RoundToInt(character.Intelligence.Value / Mathf.Pow(3, player.Defense / character.Intelligence.Value)));
        }
        return 0;
    }
    public static int MeleeDamage(Character character, int reciver, Enemys mob, PlayerManger player, int additionalDmg = 0)
    {
        switch (reciver)
        {
            case 0:
                return additionalDmg + (Mathf.RoundToInt(character.Strength.Value / Mathf.Pow(3f, mob.Defense / character.Strength.Value)));
            case 1:
                return additionalDmg + (Mathf.RoundToInt(character.Strength.Value / Mathf.Pow(3, player.Defense / character.Strength.Value)));
        }
        return 0;
    }
    public static int RangedDamage(Character character, int reciver, Enemys mob, PlayerManger player, int additionalDmg = 0)
    {
        switch (reciver)
        {
            case 0:
                return additionalDmg + (Mathf.RoundToInt(character.Strength.Value / Mathf.Pow(3f, mob.Defense / character.Strength.Value)) + Mathf.RoundToInt(character.Agility.Value * .5f));
            case 1:
                return additionalDmg + (Mathf.RoundToInt(character.Strength.Value / Mathf.Pow(3, player.Defense / character.Strength.Value)) + Mathf.RoundToInt(character.Agility.Value * .5f));
        }
        return 0;
    }
}
