using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ProjectMDataBaseEditorWindow : OdinMenuEditorWindow
{
    [MenuItem("Tools/Project M Database Editor")]
    private static void OpenWindow()
    {
        var window = GetWindow<ProjectMDataBaseEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1920, 1080);
    }

    public static SdfIcon trash;

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;
        


        tree.AddAllAssetsAtPath("Items/Equippable Items", "Assets/Prefabs/items/Items/equippible items", typeof(EquippableItem), includeSubDirectories: true).SortMenuItemsByName();
        tree.AddAllAssetsAtPath("Items/Starter Items", "Assets/Prefabs/items/Starter Gear", typeof(EquippableItem), includeSubDirectories: true).SortMenuItemsByName();
        tree.AddAllAssetsAtPath("Items/Useable Items", "Assets/Prefabs/items/Items/Usble items", typeof(UsableItem), includeSubDirectories: true).SortMenuItemsByName();
        tree.AddAllAssetsAtPath("Items/Addons/Item Effects", "Assets/Prefabs/items/Items/Usble items/Effects", typeof(UseableItemEffect), includeSubDirectories: true).SortMenuItemsByName();
        tree.AddAllAssetsAtPath("Items/Addons/Status Effects", "Assets/Scripts/Items/Status Effects", typeof(StatusEffectSO), includeSubDirectories: true).SortMenuItemsByName();
        tree.AddAllAssetsAtPath("Items/Addons/Passives", "Assets/Scripts/Items/Passives", typeof(PassiveSO), includeSubDirectories: true).SortMenuItemsByName();
        tree.AddAllAssetsAtPath("Classes", "Assets/Photon/PhotonUnityNetworking/Resources", typeof(PlayerManger)).SortMenuItemsByName().AddThumbnailIcons();
        tree.AddAllAssetsAtPath("Classes/Inventory", "Assets/Prefabs/Ui/Ui Veriations", typeof(Character)).SortMenuItemsByName().AddThumbnailIcons();
        tree.AddAllAssetsAtPath("Enemys", "Assets/Photon/PhotonUnityNetworking/Resources", typeof(Enemys)).SortMenuItemsByName().AddThumbnailIcons();
        tree.AddAllAssetsAtPath("Items/Chests", "Assets/Photon/PhotonUnityNetworking/Resources", typeof(LootContainerControl)).SortMenuItemsByName();

        tree.EnumerateTree().Where(x => x.Value as Item).ForEach(AddDragHandles);
        tree.EnumerateTree().Where(x => x.Value as PassiveSO).ForEach(AddDragHandles);
        tree.EnumerateTree().Where(x => x.Value as UseableItemEffect).ForEach(AddDragHandles);
        tree.EnumerateTree().Where(x => x.Value as StatusEffectSO).ForEach(AddDragHandles);


        tree.EnumerateTree().AddIcons<UsableItem>(x => x.Icon);
        tree.EnumerateTree().AddIcons<EquippableItem>(x => x.Icon);
        return tree;
    }
    private void AddDragHandles(OdinMenuItem menuItem)
    {
        menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
    }
    protected override void OnBeginDrawEditors()
    {
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
        var selected = this.MenuTree.Selection;

        // Draws a toolbar with the name of the currently selected menu item.
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Equippable Item")))
            {

                ScriptableObjectCreator.ShowDialog<EquippableItem>("Assets/Prefabs/items/Items/equippible items", obj =>
                {
                    obj.ItemName = obj.name;

                    base.TrySelectMenuItemWithObject(obj); 
                });
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Usable Item")))
            {
                ScriptableObjectCreator.ShowDialog<UsableItem>("Assets/Prefabs/items/Items/Usble items", obj =>
                {
                    obj.ItemName = obj.name;
                    base.TrySelectMenuItemWithObject(obj);
                });
            }
            if (selected != null)
            {
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Delete")))
                    {
                    Object assest = selected.SelectedValue as Object;
                    string path = AssetDatabase.GetAssetPath(assest);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}


public class CreateNewEquippableItem
{
    public CreateNewEquippableItem()
    {
        equippableitem = ScriptableObject.CreateInstance<EquippableItem>();
        equippableitem.name = "New Equippable Item";
    }
    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    public EquippableItem equippableitem;

    [Button("Add New Equippable Item")]
    private void CreateNewData()
    {
        AssetDatabase.CreateAsset(equippableitem, "Assets/Prefabs/items/Items/equippible items" + equippableitem.name + ".asset");
        AssetDatabase.SaveAssets();

        equippableitem = ScriptableObject.CreateInstance<EquippableItem>();
        equippableitem.name = "New Equippable Item";

    }

}
public class CreateNewUsableItem
{
    public CreateNewUsableItem()
    {
        usableitem = ScriptableObject.CreateInstance<UsableItem>();
        usableitem.name = "New Usable Item";
    }
    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    public UsableItem usableitem;

    [Button("Add New Usable Item")]
    private void CreateNewData()
    {
        AssetDatabase.CreateAsset(usableitem, "Assets/Prefabs/items/Items/Usble items" + usableitem.name + ".asset");
        AssetDatabase.SaveAssets();

        usableitem = ScriptableObject.CreateInstance<UsableItem>();
        usableitem.name = "New Usable Item";
    }

}



