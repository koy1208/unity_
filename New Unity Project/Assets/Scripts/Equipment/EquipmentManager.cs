using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager _instance = null;

    public enum EquipmentParts
    {
        NONE,
        HEAD,
        BODY,
        WEAPON,
        ARM,
        PANTS,
        BOOTS,
        PARTS_END
    }

    public enum EquipmentStat
    {
        NONE,
        HP,
        ATK,
        DEF,
        CRIDMG,
        CRIPER,
        SPEED,
        STAT_END
    }

    public enum EquipmentRarity
    {
        NONE,
        COMMON,
        UNCOMMON,
        RARE,
        EPIC
    }

    [System.Serializable]
    public class EquipmentStatValue
    {
        public EquipmentStat _stat;
        public EquipmentStat Stat { get { return _stat; } }
        public float _value;
        public float Val { get { return _value; } }

        public EquipmentStatValue()
        { }

        public EquipmentStatValue(EquipmentStat stat, float val)
        {
            _stat = stat;
            _value = val;
        }


        public float ReturnApplyValue(GameObject character)
        {
            float rtnVal = 0.0f;
            Character_Base CB = character.GetComponent<Character_Base>();


            switch (_stat)
            {
                case EquipmentStat.HP:
                    rtnVal = CB.MaxHP * _value / 100.0f;
                    break;

                case EquipmentStat.ATK:
                    rtnVal = CB.ATK * _value / 100.0f;
                    break;

                case EquipmentStat.DEF:
                    rtnVal = CB.DEF * _value / 100.0f;
                    break;

                case EquipmentStat.CRIDMG:
                case EquipmentStat.CRIPER:
                case EquipmentStat.SPEED:
                    rtnVal = _value;
                    break;
            }

            return rtnVal;
        }

        public void SetEquipState()
        {
            _stat = (EquipmentStat)Random.Range(1, (int)EquipmentStat.STAT_END);

            switch (_stat)
            {
                case EquipmentStat.HP:
                case EquipmentStat.ATK:
                case EquipmentStat.DEF:
                case EquipmentStat.CRIDMG:
                case EquipmentStat.CRIPER:
                    _value = Random.Range(4, 7);
                    break;

                case EquipmentStat.SPEED:
                    _value = Random.Range(10, 21) / 10.0f;
                    break;
            }

        }

        public void AddStatValue()
        {

            switch (_stat)
            {
                case EquipmentStat.HP:
                case EquipmentStat.ATK:
                case EquipmentStat.DEF:
                case EquipmentStat.CRIDMG:
                case EquipmentStat.CRIPER:
                    _value += Random.Range(4, 7);
                    break;

                case EquipmentStat.SPEED:
                    _value += Random.Range(10, 21) / 10.0f;
                    break;
            }
        }

    }

    int EquipmentIDCount = 0;
    

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }
    

    public void SetEquipmentID(int id)
    {
        EquipmentIDCount = id;
    }

    public Equipment_Base.Equipment_Info GetNewEquipment(EquipmentParts parts, EquipmentRarity rarity)
    {
        string EquipmentName = "";
    
        switch(parts)
        {
            case EquipmentParts.HEAD:
                EquipmentName = "Head";
                break;

            case EquipmentParts.BODY:
                EquipmentName = "Body";
                break;

            case EquipmentParts.WEAPON:
                EquipmentName = "Weapon";
                break;

            case EquipmentParts.ARM:
                EquipmentName = "Arm";
                break;

            case EquipmentParts.PANTS:
                EquipmentName = "Pants";
                break;

            case EquipmentParts.BOOTS:
                EquipmentName = "Boots";
                break;
        }

        EquipmentName += "_";
        
        switch(rarity)
        {
            case EquipmentRarity.COMMON:
                EquipmentName += "Common";
                break;

            case EquipmentRarity.UNCOMMON:
                EquipmentName += "UnCommon";
                break;

            case EquipmentRarity.RARE:
                EquipmentName += "Rare";
                break;

            case EquipmentRarity.EPIC:
                EquipmentName += "Epic";
                break;
        }


        Equipment_SO asset;
        string EassetPath = "Assets/Resources/ScriptableObject/Equipment_ScriptableObject/" + EquipmentName + ".asset";

        try
        {
            asset = (Equipment_SO)ScriptableObject.CreateInstance(typeof(Equipment_SO));
            asset = AssetDatabase.LoadAssetAtPath<Equipment_SO>(EassetPath);
        }
        catch(System.Exception e)
        {
            return null;
        }

        Equipment_Base.Equipment_Info newEquipment = new Equipment_Base.Equipment_Info();
        newEquipment.InitEquipment(asset, EquipmentIDCount);

        EquipmentIDCount++;

        return newEquipment;
    }


    public Equipment_Base.Equipment_Info GetNewRandomEquipment()
    {
        EquipmentParts randParts;
        EquipmentRarity randRarity;

        randParts = (EquipmentParts)(Random.RandomRange((int)EquipmentParts.HEAD, (int)EquipmentParts.PARTS_END));
        randRarity = (EquipmentRarity)(Random.RandomRange((int)EquipmentRarity.COMMON, (int)EquipmentRarity.EPIC + 1));

        return this.GetNewEquipment(randParts, randRarity);
    }


    public Equipment_Base.Equipment_Info GetRandomEquipmentByString(string minRarity, string maxRarity)
    {
        EquipmentRarity MinRarity;
        EquipmentRarity MaxRarity;
        EquipmentRarity RandRarity;
        EquipmentParts RandParts;

        MinRarity = GetRarityByString(minRarity);
        MaxRarity = GetRarityByString(maxRarity);


        RandRarity = (EquipmentRarity)(Random.Range((int)MinRarity, (int)MaxRarity + 1));
        RandParts = (EquipmentParts)(Random.Range((int)EquipmentParts.HEAD,
            (int)EquipmentParts.PARTS_END));

        return _instance.GetNewEquipment(RandParts, RandRarity);

    }

    public EquipmentRarity GetRarityByString(string Rarity)
    {
        EquipmentRarity rarity;

        switch (Rarity)
        {
            case "Common":
                rarity = EquipmentRarity.COMMON;
                break;

            case "UnCommon":
                rarity = EquipmentRarity.UNCOMMON;
                break;

            case "Rare":
                rarity = EquipmentRarity.RARE;
                break;

            default:
                rarity = EquipmentRarity.EPIC;
                break;
        }

        return rarity;
    }

}
