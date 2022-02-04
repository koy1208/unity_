using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager _instance = null;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    [System.Serializable]
    public class CharacterInfo
    {
        public string _characterName;
        public string CharacterName { get { return _characterName; } }

        public int _level;
        public int Level { get { return _level; } }

        public float _curExp;
        public float Exp { get { return _curExp; } }

        public float _maxExp;
        public float MaxExp { get { return _maxExp; } }

        public bool _isParty = false;
        public bool IsParty { get { return _isParty; } set { _isParty = value; } }


        public int[] _equipmentID = new int[(int)EquipmentManager.EquipmentParts.PARTS_END];
        public int[] Equipments { get { return _equipmentID; } }
       // public Equipment_Base.Equipment_Info[] _equipments = new Equipment_Base.Equipment_Info[(int)EquipmentManager.EquipmentParts.PARTS_END];
       // public Equipment_Base.Equipment_Info[] Equipments { get { return _equipments; } }

        public int _characterID = 0;
        public int ID { get { return _characterID; } }

        int _overlap;
        public int Overlap { get { return _overlap; } set { _overlap = value; } }

        public CharacterInfo(string Name, int level = 1)
        {
            _characterName = Name;
            _level = level;

            _maxExp = 1000.0f;

            _characterID = Player._instance._characterIDCount++;

            for (int i = 0; i < _equipmentID.Length; i++)
                _equipmentID[i] = -1;

            //_equipments = new Equipment_Base.Equipment_Info[(int)EquipmentManager.EquipmentParts.PARTS_END];
        }

        public bool GetExp(float exp)
        {
            _curExp += exp;

            while (_curExp >= _maxExp)
            {
                _curExp -= _maxExp;
                this.AddLevelUp(1);
                this.SetMaxExp();

                return true;

            }

            return false;
        }

        private void AddLevelUp(int Value)
        {
            _level += Value;
        }

        private void SetMaxExp()
        {
            _maxExp = (int)(_maxExp * 1.2f);
        }


        public void TestLevelUp()
        {
            _level++;
        }

        
        public void SetEquipment(Equipment_Base.Equipment_Info equipment)
        {
            if (equipment.OwnerID != -1)
                return;

           
            equipment.Wearing(_characterID);

            var curWearEquipment = Player._instance.FindEquipmentInHasEquipment(_equipmentID[(int)equipment.EquipmetInfo.Parts]);

            if (curWearEquipment != null)
                curWearEquipment.ResetOwnerID();


            _equipmentID[(int)equipment.EquipmetInfo.Parts] = equipment.ID;

            /*
            if (_equipments[(int)equipment.EquipmetInfo.Parts] != null)
                _equipments[(int)equipment.EquipmetInfo.Parts].ResetOwnerID();

            _equipments[(int)equipment.EquipmetInfo.Parts] = equipment;
            */

        }

        public void DeleteEquipment(Equipment_Base.Equipment_Info equipment)
        {
            //_equipments[(int)equipment.EquipmetInfo.Parts] = null;
            var curEquipment = Player._instance.FindEquipmentInHasEquipment(_equipmentID[(int)equipment.EquipmetInfo.Parts]);
            curEquipment.ResetOwnerID();

            _equipmentID[(int)equipment.EquipmetInfo.Parts] = -1;
        }
        

    }
}
