#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    public abstract class EquipableItem : ItemDemo
    {
        [BoxGroup(STATS_BOX_GROUP)]
        public float Durability;

        [VerticalGroup(LEFT_VERTICAL_GROUP + "/Modifiers")]
        public StatList Modifiers;
    }
}
#endif
