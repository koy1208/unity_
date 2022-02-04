using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ItemManager : MonoBehaviour
{
    public static ItemManager _inctance = null;

    public enum ITEM_TYPE
    {
        NONE,
        CHARACTER_EXP_POSION,
        EQUIPMENT_SMELTING,
        TYPE_END
    }

    public enum ITEM_RARITY
    {
        NONE,
        COMMON,
        RARE,
        RARITY_END
    }


    [System.Serializable]
    public class ItemInfo
    {
        public Item _item;
        public int _numOfItem = 0;

        public ItemInfo(Item item)
        {
            _item = item;
            _numOfItem = 1;
        }

        public void AddItem(int num)
        {
            _numOfItem += num;
        }

        public void UseItem(CharacterManager.CharacterInfo _target)
        {
            if (_item.UseItem(_target))
                _numOfItem--;

            if(_numOfItem <= 0)
            {
                Player._instance.PlayerInfo.HasItem.Remove(this);
                return;
            }
        }

        public void UseItem()
        {
            _numOfItem--;
            if (_numOfItem <= 0)
            {
                Player._instance.PlayerInfo.HasItem.Remove(this);
                return;
            }
        }

        public void UseItem(Equipment_Base.Equipment_Info _target)
        {
            if (_item.UseItem(_target))
                _numOfItem--;

            if (_numOfItem <= 0)
            {
                Player._instance.PlayerInfo.HasItem.Remove(this);
                return;
            }
        }

        public string GetName()
        {
            string assetName = "";

            switch (_item.Type)
            {
                case ITEM_TYPE.CHARACTER_EXP_POSION:
                    assetName = "CharacterExpPosion";
                    break;

                case ITEM_TYPE.EQUIPMENT_SMELTING:
                    assetName = "EquipmentSmelting";
                    break;
            }

            assetName += "_";

            switch (_item.Rarity)
            {
                case ITEM_RARITY.COMMON:
                    assetName += "Common";
                    break;

                case ITEM_RARITY.RARE:
                    assetName += "Rare";
                    break;
            }

            return assetName;
        }
    }

    private void Awake()
    {
        if(_inctance == null)
        {
            _inctance = this;
        }
    }


    public ItemInfo GetNewItem(string assetName)
    {
        Item asset;
        string assetPath = "Assets/Resources/ScriptableObject/Item_ScriptableObject/"
            + assetName + ".asset";

        try
        {
            asset = (Item)ScriptableObject.CreateInstance<Item>();
            asset = AssetDatabase.LoadAssetAtPath<Item>(assetPath);
        }
        catch(System.Exception e)
        {
            return null;
        }
        ItemInfo newItem = new ItemInfo(asset);

        return newItem;
    }

    public ItemInfo GetNewItem(ITEM_TYPE type, ITEM_RARITY rarity)
    {
        string assetName = "";

        switch(type)
        {
            case ITEM_TYPE.CHARACTER_EXP_POSION:
                assetName = "CharacterExpPosion";
                break;

            case ITEM_TYPE.EQUIPMENT_SMELTING:
                assetName = "EquipmentSmelting";
                break;
        }

        assetName += "_";

        switch(rarity)
        {
            case ITEM_RARITY.COMMON:
                assetName += "Common";
                break;

            case ITEM_RARITY.RARE:
                assetName += "Rare";
                break;
        }


        return this.GetNewItem(assetName);
    }

    public ItemInfo GetNewRandomItem()
    {
        ITEM_TYPE randType;
        ITEM_RARITY randRarity;

        //장비 강화 아이템 추가후 수정
        randType = (ITEM_TYPE)(Random.Range((int)ITEM_TYPE.CHARACTER_EXP_POSION, (int)ITEM_TYPE.EQUIPMENT_SMELTING + 1));
        randRarity = (ITEM_RARITY)(Random.Range((int)ITEM_RARITY.COMMON, (int)ITEM_RARITY.RARITY_END));

        return this.GetNewItem(randType, randRarity);
    }

    public ItemInfo GetItemByString(string itemType, string itemRarity)
    {
        string assetName = itemType + "_" + itemRarity;
        string assetPath = "Assets/Resources/ScriptableObject/Item_ScriptableObject/" + assetName +
          ".asset";

        ItemInfo newItem = _inctance.GetNewItem(assetName);


        return newItem;
    }
    
}
