using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour
{


    static public Player _instance;

    public int _characterIDCount = 0;


    JsonSingleton.SaveJsonFile _playerInfo;
    public JsonSingleton.SaveJsonFile PlayerInfo { get { return _playerInfo; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    public void InitPlayer(JsonSingleton.SaveJsonFile saveFile)
    {
        _instance._playerInfo = saveFile;

        var party = _instance._playerInfo.InPartyCharacterArray;

        for(int i= 0; i < party.Length; i++)
        {
            if (party[i] == -1)
                continue;

            var inparty = Player._instance.PlayerInfo.FindCharacterAtID(party[i]);

            if (inparty.CharacterName == "")
            {
                party[i] = -1;
                continue;
            }
            inparty.IsParty = true;

            _instance.PlayerInfo.FindCharacterAtID(party[i]).IsParty = true;
        }

        int initEquipmentID = 0;
        for (int i = 0; i < _playerInfo.HasEquipment.Count; i++)
        {
            if (initEquipmentID <= _playerInfo.HasEquipment[i].ID)
            {
                initEquipmentID = _playerInfo.HasEquipment[i].ID + 1;
            }
        }

        EquipmentManager._instance.SetEquipmentID(initEquipmentID);
    }


    public Equipment_Base.Equipment_Info FindEquipmentInHasEquipment(string ID)
    {
        for (int i = 0; i < PlayerInfo.HasEquipment.Count; i++)
        {
            if (PlayerInfo.HasEquipment[i].ID.ToString() == ID)
            {
                return PlayerInfo.HasEquipment[i];
            }
        }

        return null;
    }

    public Equipment_Base.Equipment_Info FindEquipmentInHasEquipment(int ID)
    {
        for (int i = 0; i < PlayerInfo.HasEquipment.Count; i++)
        {
            if (PlayerInfo.HasEquipment[i].ID == ID)
            {
                return PlayerInfo.HasEquipment[i];
            }
        }

        return null;
    }

    public void AddItem(ItemManager.ItemInfo newItem)
    {
        for(int i =0; i < PlayerInfo.HasItem.Count; i++)
        {
            if(PlayerInfo.HasItem[i]._item.Type == newItem._item.Type)
            {
                if (PlayerInfo.HasItem[i]._item.Rarity == newItem._item.Rarity)
                {
                    PlayerInfo.HasItem[i].AddItem(1);
                    return;
                }
            }
        }

        PlayerInfo.HasItem.Add(newItem);
    }

    public ItemManager.ItemInfo FindItemInHasItem(ItemManager.ITEM_TYPE type, ItemManager.ITEM_RARITY rarity)
    {
        for(int i =0;i <PlayerInfo.HasItem.Count; i++)
        {
            if(PlayerInfo.HasItem[i]._item.Type == type)
            {
                if(PlayerInfo.HasItem[i]._item.Rarity == rarity)
                {
                    return PlayerInfo.HasItem[i];
                }
            }
        }
    
        return null;
    }

}
