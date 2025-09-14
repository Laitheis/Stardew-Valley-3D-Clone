using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Item", menuName = "Collections/ItemTool")]
public class ToolDefinition : ItemDefinition
{
    [SerializeField] private Tool _toolType;

    public override ItemType Type => _type;

    public Tool ToolType { 
        get
        {
            if(base.Type != ItemType.Tool)
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog(
                    "Error",
                    $"{this} is not tool! Change <Item Type>.",
                    "ОК"
                );
                EditorApplication.isPlaying = false;
#endif
            }
            return _toolType;
        }
    }
    
}