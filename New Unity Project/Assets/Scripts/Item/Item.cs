using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Create_Item/MakeNewItem", order = 3)]
public class Item : ScriptableObject
{
    [SerializeField]
    Sprite _itemSprite;
    public Sprite ItemSprite { get { return _itemSprite; } }

    [SerializeField]
    ItemManager.ITEM_TYPE _itemType;
    public ItemManager.ITEM_TYPE Type { get { return _itemType; } }
  

    [SerializeField]
    ItemManager.ITEM_RARITY _itemRarity;
    public ItemManager.ITEM_RARITY Rarity { get { return _itemRarity; } }

    [SerializeField]
    float _value;
    public float Value { get { return _value; } }


    public bool UseItem(CharacterManager.CharacterInfo _target)
    {
        if (_itemType == ItemManager.ITEM_TYPE.EQUIPMENT_SMELTING)
            return false;

        _target.GetExp(_value);
        return true;
    }

    public bool UseItem(Equipment_Base.Equipment_Info _target)
    {
        if (_itemType == ItemManager.ITEM_TYPE.CHARACTER_EXP_POSION)
            return false;

        return true;
    }
}
