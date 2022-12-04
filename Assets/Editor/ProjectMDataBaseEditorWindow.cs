using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class ProjectMDataBaseEditorWindow : OdinMenuEditorWindow
{
    [MenuItem("Tools/Project M Database Editor")]
    private static void OpenWindow()
    {
        var window = GetWindow<ProjectMDataBaseEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;
   
        
        tree.AddAllAssetsAtPath("Items/Equippable Items", "Assets/Prefabs/items/Items/equippible items", typeof(EquippableItem), includeSubDirectories: true);
        tree.AddAllAssetsAtPath("Items/Useable Items", "Assets/Prefabs/items/Items/Usble items", typeof(UsableItem), includeSubDirectories: true);
        tree.AddAllAssetsAtPath("Classes", "Assets/Photon/PhotonUnityNetworking/Resources", typeof(PlayerManger));
        tree.AddAllAssetsAtPath("Enemys", "Assets/Photon/PhotonUnityNetworking/Resources", typeof(Enemys));
        tree.AddAllAssetsAtPath("Classes/Inventory", "Assets/Prefabs/Ui/Ui Veriations", typeof(Character));

        tree.EnumerateTree().Where(x => x.Value as Item).ForEach(AddDragHandles);

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
        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

        // Draws a toolbar with the name of the currently selected menu item.
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.Name);
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Equippable Item")))
            {
                ScriptableObjectCreator.ShowDialog<EquippableItem>("Assets/Prefabs/items/Items/equippible items", obj =>
                {
                    obj.ItemName = obj.name;
                    base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                });
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Usable Item")))
            {
                ScriptableObjectCreator.ShowDialog<UsableItem>("Assets/Prefabs/items/Items/Usble items", obj =>
                {
                    obj.ItemName = obj.name;
                    base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                });
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



