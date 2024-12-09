using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable/Item")]
public class ItemSO : ScriptableObject
{
    public GameObject prefab;
    public string itemName;
    public ItemType itemType;
    public Sprite itemSprite;
}

public enum ItemType {
    Gun,
    Melee,
    HealthPotion,
    ManaPotion
};
