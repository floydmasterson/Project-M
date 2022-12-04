using System.Text;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    string id;
    public string ID { get { return id; } }
    [PreviewField(60), HideLabel]
    [HorizontalGroup("Split",60)]
    public Sprite Icon;
    [VerticalGroup("Split/Right"), LabelWidth(65)]
    public string ItemName;
    [VerticalGroup("Split/Right"), LabelWidth(105)]
    [Range(1, 20)]
    public int MaximumStacks = 1;
    [VerticalGroup("Split/Right"), LabelWidth(90)]
    [ShowIf("@MaximumStacks > 1")]
    public bool IsConsumable;


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
