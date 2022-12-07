using System.Text;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string ID { get { return id; } }
    [PreviewField(60), HideLabel]
    [TableColumnWidth(300, resizable: false)]
    [HorizontalGroup("Item", 60)]
    public Sprite Icon;
    [VerticalGroup("Item/Right"), LabelWidth(65)]
    public string ItemName;
    [VerticalGroup("Item/Right"), LabelWidth(105)]
    [Range(1, 20)]
    public int MaximumStacks = 1;
    [VerticalGroup("Item/Right"), LabelWidth(90)]
    [ShowIf("@MaximumStacks > 1")]
    public bool IsConsumable;
    [PropertyOrder(8)]
    [TableColumnWidth(160, resizable: true)]
    [TextArea]
    public string Notes;
    [HideIf("@1 == 1")]
    [SerializeField] string id;


    protected static readonly StringBuilder sb = new StringBuilder();

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
    }
#endif

    public virtual Item GetCopy()
    {
        return this;
    }

    public virtual void Destroy()
    {

    }

    public virtual string GetItemType()
    {
        return "";
    }

    public virtual string GetDescription()
    {
        return "";
    }
}
