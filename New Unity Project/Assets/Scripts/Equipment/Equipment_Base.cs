using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Base : MonoBehaviour
{
    [System.Serializable]
    public class Equipment_Info
    {

        public Equipment_SO _info;
        public Equipment_SO EquipmetInfo { get { return _info; } }

        public EquipmentManager.EquipmentStatValue[] _statValues;
        public EquipmentManager.EquipmentStatValue[] StatValues { get { return _statValues; } }

        public int _equipmentID;
        public int ID { get { return _equipmentID; } set { _equipmentID = value; } }

        public int _characterID;
        public int OwnerID { get { return _characterID; } }


        public int _level;
        public int Level { get { return _level; } }

        public float _curExp;
        public float _maxExp;

        public void InitEquipment(Equipment_SO info, int ID)
        {
            if (info == null)
                return;

            _info = info;

            _equipmentID = ID;

            _characterID = -1;

            info.InitEquipment(out _statValues);

            _level = 0;
            _maxExp = 500;
        }

        public void Wearing(int CharacterID)
        {
            _characterID = CharacterID;
        }

        public void ResetOwnerID()
        {
            _characterID = -1;
        }

        public string GetName()
        {
            string assetName = "";


            switch (_info.Parts)
            {
                case EquipmentManager.EquipmentParts.HEAD:
                    assetName = "Head";
                    break;

                case EquipmentManager.EquipmentParts.BODY:
                    assetName = "Body";
                    break;

                case EquipmentManager.EquipmentParts.WEAPON:
                    assetName = "Weapon";
                    break;

                case EquipmentManager.EquipmentParts.ARM:
                    assetName = "Arm";
                    break;

                case EquipmentManager.EquipmentParts.PANTS:
                    assetName = "Pants";
                    break;

                case EquipmentManager.EquipmentParts.BOOTS:
                    assetName = "Boots";
                    break;
            }

            assetName += "_";

            switch (_info.Rarity)
            {
                case EquipmentManager.EquipmentRarity.COMMON:
                    assetName += "Common";
                    break;

                case EquipmentManager.EquipmentRarity.UNCOMMON:
                    assetName += "UnCommon";
                    break;

                case EquipmentManager.EquipmentRarity.RARE:
                    assetName += "Rare";
                    break;

                case EquipmentManager.EquipmentRarity.EPIC:
                    assetName += "Epic";
                    break;
            }

            return assetName;
        }

        public void AddExp(int value)
        {
            _curExp += value;
            
            while(_curExp >= _maxExp)
            {
                this.LevelUp();
            }
        }

        private void LevelUp()
        {
            _level++;

            _curExp -= _maxExp;
            _maxExp += (int)(_maxExp * 0.25);


            if (_level % 3 != 0)
                return;

            switch (_level)
            {
                case 6:
                    if (_statValues[1].Stat == EquipmentManager.EquipmentStat.NONE)
                    {
                        _statValues[1].SetEquipState();
                        return;
                    }
                    break;

                case 9:
                    if (_statValues[2].Stat == EquipmentManager.EquipmentStat.NONE)
                    {
                        _statValues[2].SetEquipState();
                        return;
                    }
                    break;

                case 12:
                    if (_statValues[3].Stat == EquipmentManager.EquipmentStat.NONE)
                    {
                        _statValues[3].SetEquipState();
                        return;
                    }
                    break;
            }
            
            int rnd;
            while (true)
            {
                rnd = Random.Range(0, 4);
                if (_statValues[rnd].Stat != EquipmentManager.EquipmentStat.NONE)
                {
                    _statValues[rnd].AddStatValue();
                    break;
                }
            }
        }
    }
}
