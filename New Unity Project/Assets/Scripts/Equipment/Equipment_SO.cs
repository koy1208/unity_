using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Equipment", menuName = "Create_Equipment/NewEquipment", order = 1)]
[System.Serializable]
public class Equipment_SO : ScriptableObject
{
    [SerializeField]
    public EquipmentManager.EquipmentParts _equipmentParts;
    public EquipmentManager.EquipmentParts Parts { get { return _equipmentParts; } }


    [SerializeField]
    public EquipmentManager.EquipmentRarity _equipmentRarity;
    public EquipmentManager.EquipmentRarity Rarity { get { return _equipmentRarity; } }

    [SerializeField]
    public Sprite _equipmentSource;
    public Sprite EquipmentSource { get { return _equipmentSource; } }




    private void Awake()
    {
        //_equipStatValues = new EquipmentManager.EquipmentStatValue[4];
    }

    public void InitEquipment(out EquipmentManager.EquipmentStatValue[] equipStatValues)
    {
        equipStatValues = new EquipmentManager.EquipmentStatValue[4];
        int StatNum = (int)(_equipmentRarity);

        for(int i = 0; i < StatNum; i++)
        {
            EquipmentManager.EquipmentStatValue statVal = new EquipmentManager.EquipmentStatValue();


            bool flag = true;
            while(flag)
            {
                statVal.SetEquipState();
                if (i == 0)
                    break;

                for (int j = 0; j < i; j++)
                {
                    if (equipStatValues[j].Stat == statVal.Stat)
                    {
                        flag = true;
                        break;
                    }
                    else
                        flag = false;
                }
            }
            equipStatValues[i] = statVal;
            //_equipStatValues = new EquipmentManager.EquipmentStatValue()
        }

        
    }

}
