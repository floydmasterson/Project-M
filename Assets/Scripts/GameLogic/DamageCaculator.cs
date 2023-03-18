using UnityEngine;

public static class DamageCaculator
{
    public enum Reciver
    {
        Mob,
        Player,
    }
    public static int MagicDamage(Character character, Reciver reciver, Enemys mob, PlayerManger player, int additionalDmg = 0)
    {
        switch (reciver)
        {
            case Reciver.Mob:
                return additionalDmg + (Mathf.RoundToInt(character.Intelligence.Value / Mathf.Pow(3f, mob.Defense / character.Intelligence.Value)));
            case Reciver.Player:
                return additionalDmg + (Mathf.RoundToInt(character.Intelligence.Value / Mathf.Pow(3, player.Defense / character.Intelligence.Value)));
        }
        return 0;
    }
    public static int MeleeDamage(Character character, Reciver reciver, Enemys mob, PlayerManger player, int additionalDmg = 0)
    {
        switch (reciver)
        {
            case Reciver.Mob:
                return additionalDmg + (Mathf.RoundToInt(character.Strength.Value / Mathf.Pow(3f, mob.Defense / character.Strength.Value)));
            case Reciver.Player:
                return additionalDmg + (Mathf.RoundToInt(character.Strength.Value / Mathf.Pow(3, player.Defense / character.Strength.Value)));
        }
        return 0;
    }
    public static int RangedDamage(Character character, Reciver reciver, Enemys mob, PlayerManger player, int additionalDmg = 0)
    {
        switch (reciver)
        {
            case Reciver.Mob:
                return additionalDmg + (Mathf.RoundToInt(character.Strength.Value * .8f / Mathf.Pow(3f, mob.Defense / character.Strength.Value)) + Mathf.RoundToInt(character.Agility.Value * .5f));
            case Reciver.Player:
                return additionalDmg + (Mathf.RoundToInt(character.Strength.Value * .8f / Mathf.Pow(3, player.Defense / character.Strength.Value)) + Mathf.RoundToInt(character.Agility.Value * .5f));
        }
        return 0;
    }
    public static int MobDamage(PlayerManger player, float Power, int additionalDmg = 0)
    {
        return additionalDmg + (Mathf.RoundToInt(Power / Mathf.Pow(2.2f, (player.Defense / Power))));
    }
}
