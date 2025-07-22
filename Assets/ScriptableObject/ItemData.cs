using UnityEngine;

[CreateAssetMenu(menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemId;
    public string displayName;
    public Sprite icon;
    public bool isPart;
    public ItemType itemType;
}
public enum ItemType {
    Generic,
    Part,
    Blueprint,
    Tool
}
