using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Create_Stage/MakeNewStage", order = 2)]
public class Stage_SO : ScriptableObject
{
    [SerializeField]
    string[] _enemiesName;

    [SerializeField]
    int[] _enemiesLevel;

    [SerializeField]
    string[] _enemiesInit;
    // 적 스탯 조절
    // 배열인덱스_조정할스탯_조정값
    // ex) 0_HP_10000
    // => 0번의 HP 10000으로 조정

    CharacterManager.CharacterInfo[] _enemyParty;
    public CharacterManager.CharacterInfo[] EnemyParty { get { return _enemyParty; } }

    [SerializeField]
    string[] _dropItems;
    //드랍 아이템 정보(장비 또는 아이템)
    //장비
    //ex) Equipment_Common_Rare
    // => Common ~ Rare 사이의 랜덤한 장비


    public void StageInit()
    {
        _enemyParty = new CharacterManager.CharacterInfo[4];

        for (int i = 0; i < _enemiesName.Length; i++)
        {
            if (_enemiesName[i] == "" || _enemiesName[i] == null)
                continue;

            int overlap = 0;
            for (int j = 0; j < i; j++)
            {
                if (_enemiesName[j] == "" || _enemiesName[j] == null)
                    continue;

                if (_enemyParty[j].CharacterName == _enemiesName[i])
                    overlap++;
            }
            _enemyParty[i] = new CharacterManager.CharacterInfo(_enemiesName[i], _enemiesLevel[i]);
            _enemyParty[i].Overlap = overlap;
        }


    }

    public void StageEnemyInit(ref GameObject[] enemyParty)
    {
        for (int i = 0; i < _enemiesInit.Length; i++)
        {
            string[] init = _enemiesInit[i].Split('_');

            int enemyIndex = int.Parse(init[0]);
            string initStat = init[1];
            int initValue = int.Parse(init[2]);

            //enemyParty[enemyIndex].GetComponent<Character_Base>().

            switch (initStat)
            {
                case "HP":
                    enemyParty[enemyIndex].GetComponent<Character_Base>().MaxHP = initValue;
                    break;

                case "Atk":
                    enemyParty[enemyIndex].GetComponent<Character_Base>().ATK = initValue;
                    break;

                case "Def":
                    enemyParty[enemyIndex].GetComponent<Character_Base>().DEF = initValue;
                    break;

                case "CriDMG":
                    enemyParty[enemyIndex].GetComponent<Character_Base>().CriticalDamage = initValue;
                    break;

                case "CriPer":
                    enemyParty[enemyIndex].GetComponent<Character_Base>().CriticalPercentage = initValue;
                    break;

                case "Speed":
                    enemyParty[enemyIndex].GetComponent<Character_Base>().Speed = initValue;
                    break;

            }
        }
    }
    //PlayerManager.CharacterInfo[] _enemiesInfo;

    public void ClrearStage()
    {
        this.ItemDrop();
        var playerParty = Player._instance.PlayerInfo.InPartyCharacterArray;
        foreach (var PP in playerParty)
        {
            if (PP == -1)
                continue;

            var inpartyCharacter = Player._instance.PlayerInfo.FindCharacterAtID(PP);

            if (inpartyCharacter.CharacterName == null)
                continue;

            inpartyCharacter.GetExp(200);

            //var ori = Player._instance.PlayerInfo.FindCharacterAtID(inpartyCharacter.ID);

            //Player._instance.PlayerInfo.HasCharacterList.Remove(ori);
           // Player._instance.PlayerInfo.HasCharacterList.Add(PP);
        }
    }

    private void ItemDrop()
    {
        if (_dropItems == null)
            return;

        for (int i = 0; i < _dropItems.Length; i++)
        {
            string[] itemInfo = _dropItems[i].Split('_');
            string dropItemType = itemInfo[0];
             //string itemName = itemInfo[1];

            Sprite dropItemSprite = null;

            switch (dropItemType)
            {
                case "Equipment":
                    string itemMinRarity = itemInfo[1];
                    string itemitemMaxRarity = itemInfo[2];
                    var newEquip = EquipmentManager._instance.GetRandomEquipmentByString(itemMinRarity, itemitemMaxRarity);
                    dropItemSprite = newEquip.EquipmetInfo.EquipmentSource;
                    Player._instance.PlayerInfo.HasEquipment.Add(newEquip);
                    // Inventory._instance.AddEquipment(newEquip);
                    break;
                    
                case "Item":
                    string itemType = itemInfo[1];
                    string itemRarity = itemInfo[2];
                    var newItem = ItemManager._inctance.GetItemByString(itemType, itemRarity);
                    dropItemSprite = newItem._item.ItemSprite;
                    Player._instance.AddItem(newItem);
                    break;
            }

            if (dropItemSprite != null)
            {
                Stage_UI._instance.AddDropItemSprite(dropItemSprite);
            }
        }
    }

  
    
}
