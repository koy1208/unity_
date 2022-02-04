using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LobySceneUIManager : MonoBehaviour
{
    public static LobySceneUIManager _instance = null;


  
    [SerializeField]
    Camera _uiCamera;


    [SerializeField]
    GameObject _lobyUI;
    [SerializeField]
    GameObject _charactersUI;
    [SerializeField]
    GameObject _inventoryUI;
    [SerializeField]
    GameObject _shopUI;
    [SerializeField]
    GameObject _equipmentSmeltUI;



    [SerializeField]
    Canvas _lobyUICanvas;
    [SerializeField]
    Canvas _charactersUICanvas;
    [SerializeField]
    Canvas _invenUICanvas;
    [SerializeField]
    Canvas _shopUICanvas;
    [SerializeField]
    Canvas _equipmentSmeltCanvas;


    [SerializeField]
    Text _notifyText;


    [SerializeField]
    GameObject _playerPartyPositionsObject;
    Transform[] _playerPartyPositions;

    [SerializeField]
    GameObject _stageSelectObject;

    [SerializeField]
    Image _stageSelectImage;

    ///////////////////////////////////
    CharacterManager.CharacterInfo _curSelectedCharacterInfo;
    Character_Base _curSelectCharacterBase;

    [SerializeField]
    Transform _selectedCharacterPosition;
    [SerializeField]
    ScrollRect _characterListScrollRect;
    [SerializeField]
    Button _btnSource;

    [SerializeField]
    Text _NameLvText;
    [SerializeField]
    Text _ExpText;
    [SerializeField]
    Slider _ExpBar;
    [SerializeField]
    Text _StatText;

    [SerializeField]
    GameObject _partyIndexButtons;

    [SerializeField]
    Button _removePartyButton;

    [SerializeField]
    GameObject _EquipmentImagesObject;
    Image[] _EquipmentImages;



    /////////////////////////////////////////////
    [SerializeField]
    Transform _invenSelectedCharacterPosition;

    [SerializeField]
    GameObject _invenEquipmentImagesObject;
    Image[] _invenEquipmentImages;

    [SerializeField]
    ScrollRect _inventoryScrollRect;

    [SerializeField]
    GameObject _infoUIPrefabs;
    RectTransform _infoUI;

    [SerializeField]
    Button _invenScrollEquipmentBtn;
    [SerializeField]
    Button _invenScrollItemBtn;

    Button _curInvenScrollBtn;

    [SerializeField]
    Button _invenScrollItemEventBtn;

    [SerializeField]
    Text _invenCharacterInfo_1;

    [SerializeField]
    Text _invenCharacterInfo_2;

    Equipment_Base.Equipment_Info _curSelectedEquipment = null;
    ItemManager.ItemInfo _curSelectedItem = null;
    /// <summary>
    //[SerializeField]
    RectTransform _target;

    /// </summary>
    /// 


    /////////////////////////////////////



    [SerializeField]
    Button[] _shopItemBtns;

    List<ItemManager.ItemInfo> _shopItem = new List<ItemManager.ItemInfo>();
    List<Equipment_Base.Equipment_Info> _shopEquipment = new List<Equipment_Base.Equipment_Info>();



    /// ///////////////////////////////////
    [SerializeField]
    ScrollRect _equipmentSmeltScrollView;

    [SerializeField]
    Text _smeltEquipmentInfo;

    [SerializeField]
    Image _smeltEquipmentImage;

    [SerializeField]
    GameObject _useItemList;
    Button[] _useItemBtns = null;

    Sprite _useItemBtnNoneSprite = null;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            _instance._playerPartyPositions = _instance._playerPartyPositionsObject.GetComponentsInChildren<Transform>();
        }
    }

    private void Start()
    {
        var infoUI = Instantiate(_infoUIPrefabs);
        _instance._infoUI = infoUI.GetComponent<RectTransform>();
        _curSelectedCharacterInfo = Player._instance.PlayerInfo.HasCharacterList[0];
        _curSelectCharacterBase = null;

        _curInvenScrollBtn = _invenScrollEquipmentBtn;

    }


    public void LobyUIRender()
    {
        this.ResetLobyUI();
        this.ShowPlayerPartyCharacterModel();
        this.SetStageSelectBtn();
    }

    private void ResetLobyUI()
    {
        for (int i = 1; i < _playerPartyPositions.Length; i++)
        {
            var children = _playerPartyPositions[i].GetComponentsInChildren<Transform>();

            for (int j = 1; j < children.Length; j++)
                Destroy(children[j].gameObject);
        }
    }

    private void ShowPlayerPartyCharacterModel()
    {
        var PlayerParty = Player._instance.PlayerInfo.InPartyCharacterArray;

        for (int i = 0; i < PlayerParty.Length; i++)
        {
            if (PlayerParty[i] == -1)
            {
                _playerPartyPositions[i + 1].GetComponent<Image>().enabled = true;
                continue;
            }
            var inpartyCharacter = Player._instance.PlayerInfo.FindCharacterAtID(PlayerParty[i]);
            if (inpartyCharacter.CharacterName == "" || inpartyCharacter.CharacterName == null)
                continue;


            var asset = AssetManager._instance.GetCharacter_SO(inpartyCharacter.CharacterName);

            GameObject copy = asset.CharacterSource;
            copy.transform.parent = _playerPartyPositions[i + 1];
            copy.transform.localPosition = Vector3.zero;
            copy.transform.forward = -copy.transform.forward;
            copy.transform.localScale = new Vector3(120, 120, 120);

            _playerPartyPositions[i + 1].GetComponent<Image>().enabled = false;
        }
    }


    private void SetStageSelectBtn()
    {
        _stageSelectObject.SetActive(true);
        var stageBtns = _stageSelectImage.GetComponentsInChildren<Button>();
        var stages = GameManager._instance.Stages;

        for(int i =0; i < stageBtns.Length; i++)
        {
            if(i >= stages.Length)
            {
                stageBtns[i].GetComponentInChildren<Text>().text = "Locked";
                continue;
            }

            stageBtns[i].GetComponentInChildren<Text>().text = stages[i];
            stageBtns[i].onClick.AddListener(this.onClickSelectStageBtn);
        }
        _stageSelectObject.SetActive(false);
    }



    ///////////////////////////////////////////////////////
    ///                                                 ///
    ///                   Loby                          ///
    ///             Button Event Function               ///
    ///                                                 ///
    ///                                                 ///                                                 
    ///////////////////////////////////////////////////////


    public void onClickLobyToCharacterLIstBtn()
    {
        _lobyUI.SetActive(false);
        _charactersUI.SetActive(true);
        LobyManager._instnace.SceneStateChange(LobyManager.SCENE_STATE.CHARACTER_LIST);
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }

    public void onClickLovyToInventoryBnt()
    {
        _lobyUI.SetActive(false);
        _inventoryUI.SetActive(true);
        LobyManager._instnace.SceneStateChange(LobyManager.SCENE_STATE.INVENTORY);
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }

    public void onClickLovyToShopBnt()
    {
        _lobyUI.SetActive(false);
        _shopUI.SetActive(true);
        LobyManager._instnace.SceneStateChange(LobyManager.SCENE_STATE.SHOP);
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }

    public void onClickStageBtn()
    {
        _stageSelectObject.SetActive(true);
    }

    public void onClickStageSelectObjectExit()
    {
        _stageSelectObject.SetActive(false);
    }


    public void onClickTestButton()
    {
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }

    public void onClickSelectStageBtn()
    {
        var curGameObject = EventSystem.current.currentSelectedGameObject;

        if (curGameObject.name == "Locked")
            return;

        GameManager._instance.SetStageName(curGameObject.GetComponentInChildren<Text>().text);
        SceneManager.LoadScene("StageScene");
    }


    ///////////////////////////////////////////////////////
    ///                                                 ///
    ///                                                 ///
    ///                Character List UI                ///
    ///                                                 ///
    ///                                                 ///                                                 
    ///////////////////////////////////////////////////////

    public void CharacterListUIRender()
    {
        this.ResetUI();

        _removePartyButton.gameObject.SetActive(_curSelectedCharacterInfo.IsParty);

        this.SetCurSelectedCharacterModel();
        this.SetCharacterInfo();
        this.SetCharacterScrollView();
        this.EquipmentImage();
    }

    private void ResetUI()
    {
        var layout = _characterListScrollRect.GetComponentInChildren<VerticalLayoutGroup>();
        var children = layout.GetComponentsInChildren<Transform>();

        for (int i = 1; i < children.Length; i++)
        {
            Destroy(children[i].gameObject);
        }

        if (_curSelectCharacterBase != null)
            Destroy(_curSelectCharacterBase.gameObject);
    }

    private void SetCurSelectedCharacterModel()
    {
        var asset = AssetManager._instance.GetCharacter_SO(_curSelectedCharacterInfo.CharacterName);

        GameObject copy = asset.CharacterSource;
        copy.transform.parent = _selectedCharacterPosition;
        copy.transform.localPosition = Vector3.zero;
        copy.transform.forward = -copy.transform.forward;
        copy.transform.localScale = new Vector3(150, 150, 150);

        var CB = copy.GetComponent<Character_Base>();
        CB.setInfo(_curSelectedCharacterInfo);
        CB.CharacterInit(asset, Character_Base.CHARACTER_OWNER.PLAYER);

        _curSelectCharacterBase = CB;
    }


    private void SetCharacterInfo()
    {
        _NameLvText.text = _curSelectedCharacterInfo.CharacterName + "\n"
            + "Lv" + _curSelectedCharacterInfo.Level;

        _ExpText.text = "Exp " + _curSelectedCharacterInfo.Exp + "  /  "
            + _curSelectedCharacterInfo.MaxExp;

        _ExpBar.value = Mathf.Clamp01(
            _curSelectedCharacterInfo.Exp / _curSelectedCharacterInfo.MaxExp);

        _StatText.text =
            "HP " + _curSelectCharacterBase.MaxHP +
            "\n\nAtk " + _curSelectCharacterBase.ATK +
            "\n\nDef " + _curSelectCharacterBase.DEF +
            "\n\nCriDmg " + _curSelectCharacterBase.CriticalDamage +
            "\n\nCriPer " + _curSelectCharacterBase.CriticalPercentage +
            "\n\nSpeed " + _curSelectCharacterBase.Speed;
    }


    private void SetCharacterScrollView()
    {
        var characterList = Player._instance.PlayerInfo.HasCharacterList;

        for (int i = 0; i < characterList.Count; i++)
        {
            var layout = _characterListScrollRect.GetComponentInChildren<VerticalLayoutGroup>();

            Button content = Instantiate(_btnSource);
            var scrollViewRectTransform = _characterListScrollRect.GetComponent<RectTransform>();
            var contentWidth = scrollViewRectTransform.rect.width;
            var contentHeight = scrollViewRectTransform.rect.height / 5.0f;

            var contentRectTransform = content.GetComponent<RectTransform>();
            contentRectTransform.rect.Set(0, 0,
                contentWidth, contentHeight);

            content.transform.parent = layout.transform;
            content.transform.localPosition = Vector3.zero;
            content.transform.localScale = new Vector3(1, 1, 1);

            var texts = content.GetComponentsInChildren<Text>();
            texts[1].enabled = false;

            content.name = characterList[i].CharacterName;
            var contenttext = content.GetComponentInChildren<Text>();
            contenttext.text = characterList[i].CharacterName;

            content.onClick.AddListener(this.onClickCharacterScrollBtn);
        }
    }


    private void EquipmentImage()
    {
        var EquipmentIDs = _curSelectedCharacterInfo.Equipments;
        Equipment_Base.Equipment_Info[] wearingEquipments = new Equipment_Base.Equipment_Info[(int)EquipmentManager.EquipmentParts.PARTS_END];


        for (int i = 0; i < EquipmentIDs.Length; i++)
        {
            if (EquipmentIDs[i] == -1)
                continue;

            wearingEquipments[i] = Player._instance.FindEquipmentInHasEquipment(EquipmentIDs[i]);
        }

        _EquipmentImages = _EquipmentImagesObject.GetComponentsInChildren<Image>();

        for (int i = 1; i < wearingEquipments.Length; i++)
        {
            if (wearingEquipments[i] == null)
                continue;

            if (wearingEquipments[i].EquipmetInfo == null)
                continue;

            if (wearingEquipments[i].ID == -1)
                continue;

            _EquipmentImages[i].sprite = wearingEquipments[i].EquipmetInfo.EquipmentSource;
        }
    }


    private void RemoveCurSelectedCharacterModel()
    {
        var child = _selectedCharacterPosition.GetComponentsInChildren<Transform>();


        for (int i = 1; i < child.Length; i++)
        {
            Destroy(child[i].gameObject);
        }
    }


    ///////////////////////////////////////////////////////
    ///                                                 ///
    ///                Character List                   ///
    ///             Button Event Function               ///
    ///                                                 ///
    ///                                                 ///                                                 
    ///////////////////////////////////////////////////////


    public void onClickCharacterScrollBtn()
    {
        GameObject curGameObject = EventSystem.current.currentSelectedGameObject;
        string btnName = curGameObject.GetComponentInChildren<Text>().text;

        _curSelectedCharacterInfo = Player._instance.PlayerInfo.FindCharacterAtName(btnName);
        this.CharacterListUIRender();
    }

    public void onClickInPartyButton()
    {
        _partyIndexButtons.SetActive(true);
    }

    public void onClickPartyIndexButton()
    {
        GameObject curObject = EventSystem.current.currentSelectedGameObject;
        var partyIdx = curObject.GetComponentInChildren<Text>().text;

        Player._instance.PlayerInfo.AddCharacterAtParty(_curSelectedCharacterInfo, int.Parse(partyIdx) - 1);

        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
        _partyIndexButtons.SetActive(false);

        this.CharacterListUIRender();

        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }

    public void onClickRemovePartyButton()
    {
        Player._instance.PlayerInfo.RemoveCharacterAtParty(_curSelectedCharacterInfo);

        this.CharacterListUIRender();

        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }

    public void onClickCharacterListToLobyButton()
    {
        _lobyUI.SetActive(true);
        _charactersUI.SetActive(false);
        LobyManager._instnace.SceneStateChange(LobyManager.SCENE_STATE.LOBY);
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }


    ///////////////////////////////////////////////////////
    ///                                                 ///
    ///                                                 ///
    ///                   Inventory UI                  ///
    ///                                                 ///
    ///                                                 ///                                                 
    ///////////////////////////////////////////////////////

    public void InventoryUIRender()
    {
        _curSelectedEquipment = null;
        _curSelectedItem = null;
        _curInvenScrollBtn.GetComponent<BtnOnOff>().setBtnOnOff(BtnOnOff.OnOff.ON);
        this.SetCurSelectedCharacterModelInInven();
        this.SetInvenEquipmentImage();
        this.SetInvenScrollView();
        this.SetInvenCharacterInfo();
    }

    private void SetCurSelectedCharacterModelInInven()
    {
        if (_curSelectCharacterBase != null)
        {
            Destroy(_curSelectCharacterBase.gameObject);
        }

        var asset = AssetManager._instance.GetCharacter_SO(_curSelectedCharacterInfo.CharacterName);

        GameObject copy = asset.CharacterSource;
        copy.transform.parent = _invenSelectedCharacterPosition;
        copy.transform.localPosition = Vector3.zero;
        copy.transform.forward = -copy.transform.forward;
        copy.transform.localScale = new Vector3(150, 150, 150);

        var CB = copy.GetComponent<Character_Base>();
        CB.setInfo(_curSelectedCharacterInfo);
        CB.CharacterInit(asset, Character_Base.CHARACTER_OWNER.PLAYER);

        _curSelectCharacterBase = CB;
    }

    private void SetInvenEquipmentImage()
    {
        var EquipmentIDs = _curSelectedCharacterInfo.Equipments;
        Equipment_Base.Equipment_Info[] wearingEquipments = new Equipment_Base.Equipment_Info[(int)EquipmentManager.EquipmentParts.PARTS_END];


        for (int i = 0; i < EquipmentIDs.Length; i++)
        {
            if (EquipmentIDs[i] == -1)
                continue;

            wearingEquipments[i] = Player._instance.FindEquipmentInHasEquipment(EquipmentIDs[i]);
        }

        _invenEquipmentImages = _invenEquipmentImagesObject.GetComponentsInChildren<Image>();

        for (int i = 1; i < wearingEquipments.Length; i++)
        {
            if (wearingEquipments[i] == null)
                continue;

            if (wearingEquipments[i].EquipmetInfo == null)
                continue;

            if (wearingEquipments[i].ID == -1)
                continue;

            _invenEquipmentImages[i].sprite = wearingEquipments[i].EquipmetInfo.EquipmentSource;
        }
    }


    private void SetInvenScrollView()
    {
        this.InvenScrollReset();
        _invenScrollItemEventBtn.onClick.RemoveAllListeners();
        if (_curInvenScrollBtn == _invenScrollEquipmentBtn)
        {
            this.EquipmentInven();
            _invenScrollItemEventBtn.GetComponentInChildren<Text>().text = "Wear";
            _invenScrollItemEventBtn.onClick.AddListener(this.onClickWearEquipmentBtn);
        }
        else
        {
            this.ItemIven();
            _invenScrollItemEventBtn.GetComponentInChildren<Text>().text = "Use";
            _invenScrollItemEventBtn.onClick.AddListener(this.onClickUseItemBtn);
        }
    }

    private void EquipmentInven()
    {
        var equipments = Player._instance.PlayerInfo.HasEquipment;

        var layout = _inventoryScrollRect.GetComponentInChildren<GridLayoutGroup>();

        for (int i = 0; i < equipments.Count; i++)
        {

            Button content = Instantiate(_btnSource);
            var scrollViewRectTransform = _characterListScrollRect.GetComponent<RectTransform>();

            content.transform.parent = layout.transform;
            content.transform.localPosition = Vector3.zero;
            content.transform.localScale = new Vector3(1, 1, 1);

            var texts = content.GetComponentsInChildren<Text>();
            texts[0].enabled = false;
            texts[1].enabled = false;

            if (equipments[i].Level > 0)
            {
                texts[2].enabled = true;
                texts[2].text = equipments[i].Level.ToString();
            }

            content.name = "Equipment_" + equipments[i].ID.ToString();

            if (equipments[i].OwnerID != -1)
            {
                var trans = content.GetComponentsInChildren<Transform>();
                if (trans.Length > 1)
                {
                    trans[2].GetComponent<Image>().enabled = true;
                }
            }
            
            var contentImage = content.GetComponent<Image>().sprite =
                equipments[i].EquipmetInfo.EquipmentSource;
        }
    }

    private void ItemIven()
    {
        var items = Player._instance.PlayerInfo.HasItem;

        var layout = _inventoryScrollRect.GetComponentInChildren<GridLayoutGroup>();

        for (int i = 0; i < items.Count; i++)
        {

            Button content = Instantiate(_btnSource);
            var scrollViewRectTransform = _characterListScrollRect.GetComponent<RectTransform>();

            content.transform.parent = layout.transform;
            content.transform.localPosition = Vector3.zero;
            content.transform.localScale = new Vector3(1, 1, 1);

            var texts = content.GetComponentsInChildren<Text>();
            texts[0].enabled = false;
            texts[1].text = items[i]._numOfItem.ToString();

            content.name = "Item_";

            switch (items[i]._item.Type)
            {
                case ItemManager.ITEM_TYPE.CHARACTER_EXP_POSION:
                    content.name += "CharacterExpPosion";
                    break;

                case ItemManager.ITEM_TYPE.EQUIPMENT_SMELTING:
                    content.name += "EquipmentSmelting";
                    break;
            }

            switch (items[i]._item.Rarity)
            {
                case ItemManager.ITEM_RARITY.COMMON:
                    content.name += "_Common";
                    break;

                case ItemManager.ITEM_RARITY.RARE:
                    content.name += "_Rare";
                    break;
            }

            var contentImage = content.GetComponent<Image>().sprite =
                items[i]._item.ItemSprite;
        }
    }


    private void InvenScrollReset()
    {
        var layout = _inventoryScrollRect.GetComponentInChildren<GridLayoutGroup>();

        var contents = layout.GetComponentsInChildren<Transform>();
        for (int i = 1; i < contents.Length; i++)
        {
            Destroy(contents[i].gameObject);
        }
    }




    public void ResetLeftClickEvent()
    {
        _infoUI.gameObject.SetActive(false);
    }

    private void SetInvenCharacterInfo()
    {
        _invenCharacterInfo_1.text = "Lv: " + _curSelectedCharacterInfo.Level + "\n\n"
            + "HP: " + _curSelectCharacterBase.MaxHP + "\n\n"
            + "Atk: " + _curSelectCharacterBase.ATK + "\n\n"
            + "Def: " + _curSelectCharacterBase.DEF;

        _invenCharacterInfo_2.text = "Exp " + _curSelectedCharacterInfo.Exp + "  /  "
            + _curSelectedCharacterInfo.MaxExp + "\n\n"
            + "CriDmg: " + _curSelectCharacterBase.CriticalDamage + "\n\n"
            + "CriPer: " + _curSelectCharacterBase.CriticalPercentage + "\n\n"
            + "Speed : " + _curSelectCharacterBase.Speed;
    }


    ///////////////////////////////////////////////////////
    ///                                                 ///
    ///                 Inventory UI                    ///
    ///             Button Event Function               ///
    ///                                                 ///
    ///                                                 ///                                                 
    ///////////////////////////////////////////////////////


    public void onClickWearEquipmentBtn()
    {
        if (_curSelectedEquipment == null)
            return;

        if (_curSelectedCharacterInfo == null)
            return;

        CharacterManager.CharacterInfo ownerInfo = Player._instance.
            PlayerInfo.FindCharacterAtID(_curSelectedEquipment.OwnerID);
        if (ownerInfo != null)
        {
            ownerInfo.DeleteEquipment(_curSelectedEquipment);
            _curSelectedEquipment.ResetOwnerID();
        }

        _curSelectedCharacterInfo.SetEquipment(_curSelectedEquipment);

        this.InventoryUIRender();

        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }


    public void onClickUseItemBtn()
    {
        if (_curSelectedItem == null)
            return;

        if (_curSelectedCharacterInfo == null)
            return;

        _curSelectedItem.UseItem(_curSelectedCharacterInfo);

        this.InventoryUIRender();

        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }

    public void onClickBackBtn()
    {
        _lobyUI.SetActive(true);
        _inventoryUI.SetActive(false);
        LobyManager._instnace.SceneStateChange(LobyManager.SCENE_STATE.LOBY);
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);

    }

    public void onClickInvenToEquipmentSmelt()
    {
        if(_curSelectedEquipment == null)
        {
            StartCoroutine(this.SetNotifyText("선택된 장비가 없습니다"));
            return;
        }

        _equipmentSmeltUI.SetActive(true);
        _inventoryUI.SetActive(false);
        LobyManager._instnace.SceneStateChange(LobyManager.SCENE_STATE.EQUIPMENT_SMELT);
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }

    public void onClickInvenScrollChangeBtn()
    {
        var curObject = EventSystem.current.currentSelectedGameObject;

        var onOff = curObject.GetComponent<BtnOnOff>();
        if (onOff == null)
            return;

        if (_curInvenScrollBtn == curObject.GetComponent<Button>())
            return;

        _curInvenScrollBtn.GetComponent<BtnOnOff>().setBtnOnOff(BtnOnOff.OnOff.OFF);

        if (_curInvenScrollBtn == _invenScrollEquipmentBtn)
            _curInvenScrollBtn = _invenScrollItemBtn;
        else
            _curInvenScrollBtn = _invenScrollEquipmentBtn;

        _curInvenScrollBtn.GetComponent<BtnOnOff>().setBtnOnOff(BtnOnOff.OnOff.ON);
        this.SetInvenScrollView();
    }



    ///////////////////////////////////////////////////////
    ///                                                 ///
    ///                                                 ///
    ///                    Shop UI                      ///
    ///                                                 ///
    ///                                                 ///                                                 
    ///////////////////////////////////////////////////////

    public void ShopUIRender()
    {
        _curSelectedItem = null;
        _curSelectedEquipment = null;

        this.ResetShopItems();
        if (_shopEquipment.Count == 0 && _shopItem.Count == 0)
            this.onClickRefreshShopItemBtn();
    }

    private Equipment_Base.Equipment_Info AddShopEquipment()
    {
        Equipment_Base.Equipment_Info newRandomEquipment = null;

        while (newRandomEquipment == null)
            newRandomEquipment = EquipmentManager._instance.GetNewRandomEquipment();

        _shopEquipment.Add(newRandomEquipment);
        return newRandomEquipment;
    }

    private ItemManager.ItemInfo AddShopItemAnd()
    {
        ItemManager.ItemInfo newRandomItem = null;

        while (newRandomItem == null)
            newRandomItem = ItemManager._inctance.GetNewRandomItem();

        _shopItem.Add(newRandomItem);
        return newRandomItem;
    }

    private void SetRandomItem(Button btn)
    {
        int randVal;
        Sprite btnSprite = null;
        string BtnName = "";

        randVal = Random.Range(0, 2);

        switch (randVal)
        {
            case 0:
                var newEquipment = this.AddShopEquipment();
                btnSprite = newEquipment._info.EquipmentSource;
                BtnName = "Equipment_" + newEquipment.ID.ToString();//newEquipment.GetName();
                break;

            case 1:
                var newItem = this.AddShopItemAnd();
                btnSprite = newItem._item.ItemSprite;
                BtnName = "Item_" + newItem.GetName();
                break;
        }

        btn.name = BtnName;
        btn.GetComponent<Image>().sprite = btnSprite;
    }

    private void ResetShopItems()
    {
        for (int i = 0; i < _shopItem.Count; i++)
        {
            _shopItem.Remove(_shopItem[i]);
            i--;
        }

        for (int i = 0; i < _shopEquipment.Count; i++)
        {
            _shopEquipment.Remove(_shopEquipment[i]);
            i--;
        }
    }


    ///////////////////////////////////////////////////////
    ///                                                 ///
    ///                    Shop UI                      ///
    ///             Button Event Function               ///
    ///                                                 ///
    ///                                                 ///                                                 
    ///////////////////////////////////////////////////////



    public void onClickRefreshShopItemBtn()
    {
        int randVal;
        Sprite btnSprite = null;
        string BtnName = "";

        this.ResetShopItems();
        for (int i = 0; i < _shopItemBtns.Length; i++)
        {
            this.SetRandomItem(_shopItemBtns[i]);
        }
    }

    public void onClickBuyItemBtn()
    {
        string btnName = "";
        if(_curSelectedItem != null)
        {
            Player._instance.AddItem(_curSelectedItem);
            btnName = "Item_" + _curSelectedItem.GetName();
            _curSelectedItem = null;
        }
        else
        {
            Player._instance.PlayerInfo.HasEquipment.Add(_curSelectedEquipment);
            btnName = "Equipment_" + _curSelectedEquipment.ID.ToString();
            _curSelectedEquipment = null;
        }

        Debug.Log(btnName);

        for(int i =0; i <_shopItemBtns.Length; i++)
        {
            if(_shopItemBtns[i].name == btnName)
            {
                this.SetRandomItem(_shopItemBtns[i]);
                break;
            }
        }

    }


    public void onClickShopToLobyBnt()
    {
        _lobyUI.SetActive(true);
        _shopUI.SetActive(false);
        LobyManager._instnace.SceneStateChange(LobyManager.SCENE_STATE.LOBY);
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }

    public void onClickShopToInvenBnt()
    {
        _inventoryUI.SetActive(true);
        _shopUI.SetActive(false);
        LobyManager._instnace.SceneStateChange(LobyManager.SCENE_STATE.INVENTORY);
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }


    ///////////////////////////////////////////////////////
    ///                                                 ///
    ///                                                 ///
    ///                EquipmentSmelt UI                ///
    ///                                                 ///
    ///                                                 ///                                                 
    ///////////////////////////////////////////////////////

    public void EquipmentSmeltUIRender()
    {
        if (_useItemBtns == null)
        {
            _useItemBtns = _useItemList.GetComponentsInChildren<Button>();
            for(int i =0; i< _useItemBtns.Length; i++)
            {
                _useItemBtns[i].onClick.AddListener(this.onClickUseSmeltItemListBtn);
            }
        }

        if(_useItemBtnNoneSprite == null)
        {
            _useItemBtnNoneSprite = _useItemBtns[0].GetComponent<Image>().sprite;
        }

        this.ResetSmeltItemScroll();
        this.SetEquipmentImage();
        this.SetSmeltItemScroll();
        this.SetSmeltEquipmentInfo();
    }

    private void SetEquipmentImage()
    {
        _smeltEquipmentImage.sprite = _curSelectedEquipment._info.EquipmentSource;
        if(_curSelectedEquipment.Level > 0)
        {
            var text = _smeltEquipmentImage.GetComponentInChildren<Text>();
            text.enabled = true;
            text.text = _curSelectedEquipment.Level.ToString();
        }
        else
        {
            var text = _smeltEquipmentImage.GetComponentInChildren<Text>();
            text.enabled = false;
        }

    }

    private void SetSmeltItemScroll()
    {
        var items = Player._instance.PlayerInfo.HasItem;

        var layout = _equipmentSmeltScrollView.GetComponentInChildren<GridLayoutGroup>();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i]._item.Type != ItemManager.ITEM_TYPE.EQUIPMENT_SMELTING)
                continue;

            Button content = Instantiate(_btnSource);
            var scrollViewRectTransform = _characterListScrollRect.GetComponent<RectTransform>();

            content.transform.parent = layout.transform;
            content.transform.localPosition = Vector3.zero;
            content.transform.localScale = new Vector3(1, 1, 1);
            content.onClick.AddListener(this.onClickSmeltItemScrollBtn);

            var texts = content.GetComponentsInChildren<Text>();
            texts[0].enabled = false;
            texts[1].text = items[i]._numOfItem.ToString();

            //content.name = "Item_";



            switch (items[i]._item.Type)
            {
                case ItemManager.ITEM_TYPE.EQUIPMENT_SMELTING:
                    content.name = "EquipmentSmelting";
                    break;
            }

            switch (items[i]._item.Rarity)
            {
                case ItemManager.ITEM_RARITY.COMMON:
                    content.name += "_Common";
                    break;

                case ItemManager.ITEM_RARITY.RARE:
                    content.name += "_Rare";
                    break;
            }

            var contentImage = content.GetComponent<Image>().sprite =
                items[i]._item.ItemSprite;
        }
    }

    private void ResetSmeltItemScroll()
    {
        var layout = _equipmentSmeltScrollView.GetComponentInChildren<GridLayoutGroup>();
        var children = layout.GetComponentsInChildren<Transform>();

        for (int i = 1; i < children.Length; i++)
        {
            Destroy(children[i].gameObject);
        }
    }

    private void SetSmeltEquipmentInfo()
    {
        string OptText;
        string PartsStr = "Parts: ";
        string RarityStr = "Rarity: ";

        if (_curSelectedEquipment == null)
            return;

        EquipmentManager.EquipmentParts equipParts;
        equipParts = _curSelectedEquipment.EquipmetInfo.Parts;


        OptText = "Lv: " + _curSelectedEquipment.Level.ToString();
        OptText += "\nExp: " + _curSelectedEquipment._curExp +  " / " +
            _curSelectedEquipment._maxExp;


        int plusExp = 0;
        
        for(int i =0; i < _useItemBtns.Length; i++)
        {
            var btnName = _useItemBtns[i].name;
            if (btnName == "None")
                break;

            var itemInfo = btnName.Split('_');
            
            switch(itemInfo[1])
            {
                case "Common":
                    plusExp += (int)ItemManager._inctance.GetNewItem("EquipmentSmelting_Common")._item.Value;

                    break;

                case "Rare":
                    plusExp += (int)ItemManager._inctance.GetNewItem("EquipmentSmelting_Rare")._item.Value;
                    break;
            }

        }

        if(plusExp > 0)
        {
            OptText += "  (" + plusExp + ")";
        }

        OptText += "\n";

        switch (equipParts)
        {
            case EquipmentManager.EquipmentParts.HEAD:
                PartsStr += "Head";
                break;

            case EquipmentManager.EquipmentParts.BODY:
                PartsStr += "Body";
                break;

            case EquipmentManager.EquipmentParts.WEAPON:
                PartsStr += "Weapon";
                break;

            case EquipmentManager.EquipmentParts.ARM:
                PartsStr += "Arm";
                break;

            case EquipmentManager.EquipmentParts.PANTS:
                PartsStr += "Pants";
                break;

            case EquipmentManager.EquipmentParts.BOOTS:
                PartsStr += "Boots";
                break;
        }

        OptText += PartsStr + "\n";



        EquipmentManager.EquipmentRarity rarity = _curSelectedEquipment.EquipmetInfo.Rarity;

        switch (rarity)
        {
            case EquipmentManager.EquipmentRarity.COMMON:
                RarityStr += "Common";
                break;

            case EquipmentManager.EquipmentRarity.UNCOMMON:
                RarityStr += "UnCommon";
                break;

            case EquipmentManager.EquipmentRarity.RARE:
                RarityStr += "Rare";
                break;

            case EquipmentManager.EquipmentRarity.EPIC:
                RarityStr += "Epic";
                break;
        }

        OptText += "\n" + RarityStr + "\n";


        string SubOtpStr;
        EquipmentManager.EquipmentStatValue[] statVals = _curSelectedEquipment.StatValues;

        for (int i = 0; i < statVals.Length; i++)
        {
            if (statVals[i] == null)
                break;

            if (statVals[i].Stat == EquipmentManager.EquipmentStat.NONE)
                break;

            SubOtpStr = "Sub_Opt" + (i + 1) + ": ";

            switch (statVals[i].Stat)
            {
                case EquipmentManager.EquipmentStat.HP:
                    SubOtpStr += "HP: ";
                    break;

                case EquipmentManager.EquipmentStat.ATK:
                    SubOtpStr += "ATK: ";
                    break;

                case EquipmentManager.EquipmentStat.DEF:
                    SubOtpStr += "DEF: ";
                    break;

                case EquipmentManager.EquipmentStat.CRIDMG:
                    SubOtpStr += "CRIDMG: ";
                    break;

                case EquipmentManager.EquipmentStat.CRIPER:
                    SubOtpStr += "CRIPER: ";
                    break;

                case EquipmentManager.EquipmentStat.SPEED:
                    SubOtpStr += "SPD: ";
                    break;
            }

            SubOtpStr += statVals[i].Val.ToString();
            OptText += "\n" + SubOtpStr;
        }

        if (_curSelectedEquipment.OwnerID != -1)
        {
            CharacterManager.CharacterInfo OwnerInfo = Player._instance.PlayerInfo.FindCharacterAtID(_curSelectedEquipment.OwnerID);
            OptText += "\n\nOwner: " + OwnerInfo.CharacterName;//+ equipment.OwnerID.ToString();
        }

        _smeltEquipmentInfo.GetComponentInChildren<Text>().text = OptText;
    }


    ///////////////////////////////////////////////////////
    ///                                                 ///
    ///                                                 ///
    ///                EquipmentSmelt UI                ///
    ///             Button Event Function               ///
    ///                                                 ///                                                 
    ///////////////////////////////////////////////////////

    public void onClickSmeltItemScrollBtn()
    {
        GameObject curGameObject = EventSystem.current.currentSelectedGameObject;
        var btnName = curGameObject.name;
        var item = ItemManager._inctance.GetNewItem(btnName);
        var btnNameStr = btnName.Split('_');

        ItemManager.ITEM_RARITY rarity = ItemManager.ITEM_RARITY.NONE;



        for (int i = 0; i < _useItemBtns.Length; i++)
        {
            if (_useItemBtns[i].name != "None")
            {
                if (i == _useItemBtns.Length - 1)
                    return;
                continue;
            }
            _useItemBtns[i].GetComponent<Image>().sprite = item._item.ItemSprite;
            _useItemBtns[i].name = btnName;
            break;
        }

        switch (btnNameStr[1])
        {
            case "Common":
                rarity = ItemManager.ITEM_RARITY.COMMON;
                break;

            case "Rare":
                rarity = ItemManager.ITEM_RARITY.RARE;
                break;
        }

        if (rarity == ItemManager.ITEM_RARITY.NONE)
            return;

        var playerItem = Player._instance.FindItemInHasItem(ItemManager.ITEM_TYPE.EQUIPMENT_SMELTING, rarity);
        playerItem.UseItem();



        this.EquipmentSmeltUIRender();
    }

    public void onClickUseSmeltItemListBtn()
    {
        GameObject curGameObject = EventSystem.current.currentSelectedGameObject;
        var btnName = curGameObject.name;
        if (btnName == "None")
            return;


        ItemManager.ItemInfo rtnItem = ItemManager._inctance.GetNewItem(btnName);
        Player._instance.AddItem(rtnItem);

        int idx = -1;
        for(int i = 0; i < _useItemBtns.Length; i++)
        {
            if (_useItemBtns[i].gameObject == curGameObject)
            {
                idx = i;
                break;
            }
        }

        if (idx == -1)
            return;

        for(int i = idx; i < _useItemBtns.Length; i++)
        {
            if(i == _useItemBtns.Length - 1)
            {
                _useItemBtns[i].GetComponent<Image>().sprite = _useItemBtnNoneSprite;
                _useItemBtns[i].name = "None";
                break;
            }

            _useItemBtns[i].GetComponent<Image>().sprite = _useItemBtns[i + 1].GetComponent<Image>().sprite;
            _useItemBtns[i].name = _useItemBtns[i + 1].name;
        }

        this.EquipmentSmeltUIRender();

    }

    public void onClickSmeltEquipment()
    {
        if (_useItemBtns[0].name == "None")
        {
            StartCoroutine(this.SetNotifyText("강화아이템을 선택하십시오"));
            return;
        }
        int AddExp = 0;

        for(int i = 0; i < _useItemBtns.Length; i++)
        {
            var btnName = _useItemBtns[i].name;
            if (btnName == "None")
                continue;

            var itemInfo = btnName.Split('_');

            switch (itemInfo[1])
            {
                case "Common":
                    AddExp += (int)ItemManager._inctance.GetNewItem("EquipmentSmelting_Common")._item.Value;

                    break;

                case "Rare":
                    AddExp += (int)ItemManager._inctance.GetNewItem("EquipmentSmelting_Rare")._item.Value;
                    break;
            }

            _useItemBtns[i].name = "None";
            _useItemBtns[i].GetComponent<Image>().sprite = _useItemBtnNoneSprite;
        }

        _curSelectedEquipment.AddExp(AddExp);

        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
        this.EquipmentSmeltUIRender();
    }

    public void onClickEquipmentSmeltToInven()
    {
        _inventoryUI.SetActive(true);
        _equipmentSmeltUI.SetActive(false);
        LobyManager._instnace.SceneStateChange(LobyManager.SCENE_STATE.INVENTORY);
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
    }


    ///////////////////////////////////////////////////////
    ///                                                 ///
    ///                                                 ///
    ///                     공용 함수                   ///
    ///                                                 ///
    ///                                                 ///                                                 
    ///////////////////////////////////////////////////////

    public void ShowItemInfo(string name, LobyManager.SCENE_STATE scene)
    {
        ItemManager.ItemInfo item = null;
        ItemManager.ItemInfo asset = null;
        switch (scene)
        {
            case LobyManager.SCENE_STATE.INVENTORY:
                asset = ItemManager._inctance.GetNewItem(name);
                item = Player._instance.FindItemInHasItem(asset._item.Type, asset._item.Rarity);
                break;

            case LobyManager.SCENE_STATE.SHOP:
                asset = ItemManager._inctance.GetNewItem(name);
                for (int i =0; i < _shopItem.Count; i++)
                {
                    if(_shopItem[i]._item.Type == asset._item.Type)
                    {
                        if(_shopItem[i]._item.Rarity == asset._item.Rarity)
                        {
                            item = _shopItem[i];
                            break;
                        }
                    }
                }
                break;
        }

        _instance.SetItemInfoUI(item, scene);
    }


    public void ShowEquipmentInfo(string id, LobyManager.SCENE_STATE scene)
    {
        Equipment_Base.Equipment_Info equipment = null;

        switch (scene)
        {
            case LobyManager.SCENE_STATE.INVENTORY:
                equipment = Player._instance.FindEquipmentInHasEquipment(id);
                break;

            case LobyManager.SCENE_STATE.SHOP:
                for(int i =0; i < _shopEquipment.Count; i++)
                {
                    if (_shopEquipment[i].ID == int.Parse(id))
                    {
                        equipment = _shopEquipment[i];
                        break;
                    }
                }
                break;
        }

        _instance.SetEquipmentInfoUI(equipment, scene);
    }

    private void SetEquipmentInfoUI(Equipment_Base.Equipment_Info equipment, LobyManager.SCENE_STATE scene)
    {
        _curSelectedEquipment = equipment;
        _curSelectedItem = null;


        Vector2 ScreenPoint;
      
        if (RectTransformUtility.RectangleContainsScreenPoint(_target, Input.mousePosition, _uiCamera))
        {
            _infoUI.gameObject.SetActive(true);

            switch (scene)
            {
                case LobyManager.SCENE_STATE.CHARACTER_LIST:
                    _infoUI.transform.parent = _charactersUI.transform;

                    break;

                case LobyManager.SCENE_STATE.INVENTORY:
                    _infoUI.transform.parent = _invenUICanvas.transform;
                    break;

                case LobyManager.SCENE_STATE.SHOP:
                    _infoUI.transform.parent = _shopUICanvas.transform;
                    break;
            }

            _infoUI.transform.localPosition = Vector3.zero;
            _infoUI.transform.localScale = new Vector3(1, 1, 1);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_target, Input.mousePosition, _uiCamera, out ScreenPoint);
            _infoUI.localPosition = ScreenPoint;

            float uiPos = _infoUI.localPosition.x + _infoUI.rect.width;
            if (Screen.width / 2 <= uiPos)
                _infoUI.localPosition = new Vector2(_infoUI.localPosition.x - _infoUI.rect.width, _infoUI.localPosition.y);

            string OptText;
            string PartsStr = "Parts: ";
            string RarityStr = "Rarity: ";

            if (equipment == null)
                return;

            EquipmentManager.EquipmentParts equipParts;
            equipParts = equipment.EquipmetInfo.Parts;

            switch (equipParts)
            {
                case EquipmentManager.EquipmentParts.HEAD:
                    PartsStr += "Head";
                    break;

                case EquipmentManager.EquipmentParts.BODY:
                    PartsStr += "Body";
                    break;

                case EquipmentManager.EquipmentParts.WEAPON:
                    PartsStr += "Weapon";
                    break;

                case EquipmentManager.EquipmentParts.ARM:
                    PartsStr += "Arm";
                    break;

                case EquipmentManager.EquipmentParts.PANTS:
                    PartsStr += "Pants";
                    break;

                case EquipmentManager.EquipmentParts.BOOTS:
                    PartsStr += "Boots";
                    break;
            }

            OptText = PartsStr + "\n";



            EquipmentManager.EquipmentRarity rarity = equipment.EquipmetInfo.Rarity;

            switch (rarity)
            {
                case EquipmentManager.EquipmentRarity.COMMON:
                    RarityStr += "Common";
                    break;

                case EquipmentManager.EquipmentRarity.UNCOMMON:
                    RarityStr += "UnCommon";
                    break;

                case EquipmentManager.EquipmentRarity.RARE:
                    RarityStr += "Rare";
                    break;

                case EquipmentManager.EquipmentRarity.EPIC:
                    RarityStr += "Epic";
                    break;
            }

            OptText += "\n" + RarityStr + "\n";


            string SubOtpStr;
            EquipmentManager.EquipmentStatValue[] statVals = equipment.StatValues;

            for (int i = 0; i < statVals.Length; i++)
            {
                if (statVals[i] == null)
                    break;

                if (statVals[i].Stat == EquipmentManager.EquipmentStat.NONE)
                    break;

                SubOtpStr = "Sub_Opt" + (i + 1) + ": ";

                switch (statVals[i].Stat)
                {
                    case EquipmentManager.EquipmentStat.HP:
                        SubOtpStr += "HP: ";
                        break;

                    case EquipmentManager.EquipmentStat.ATK:
                        SubOtpStr += "ATK: ";
                        break;

                    case EquipmentManager.EquipmentStat.DEF:
                        SubOtpStr += "DEF: ";
                        break;

                    case EquipmentManager.EquipmentStat.CRIDMG:
                        SubOtpStr += "CRIDMG: ";
                        break;

                    case EquipmentManager.EquipmentStat.CRIPER:
                        SubOtpStr += "CRIPER: ";
                        break;

                    case EquipmentManager.EquipmentStat.SPEED:
                        SubOtpStr += "SPD: ";
                        break;
                }

                SubOtpStr += statVals[i].Val.ToString();
                OptText += "\n" + SubOtpStr;
            }

            if (equipment.OwnerID != -1)
            {
                CharacterManager.CharacterInfo OwnerInfo = Player._instance.PlayerInfo.FindCharacterAtID(equipment.OwnerID);
                OptText += "\n\nOwner: " + OwnerInfo.CharacterName;//+ equipment.OwnerID.ToString();
            }

            _infoUI.GetComponentInChildren<Text>().text = OptText;

        }
    }


    private void SetItemInfoUI(ItemManager.ItemInfo item, LobyManager.SCENE_STATE scene)
    {
        _curSelectedItem = item;
        _curSelectedEquipment = null;

        _infoUI.gameObject.SetActive(true);

        switch (scene)
        {
            case LobyManager.SCENE_STATE.CHARACTER_LIST:
                _infoUI.transform.parent = _charactersUI.transform;

                break;

            case LobyManager.SCENE_STATE.INVENTORY:
                _infoUI.transform.parent = _invenUICanvas.transform;
                break;

            case LobyManager.SCENE_STATE.SHOP:
                _infoUI.transform.parent = _shopUICanvas.transform;
                break;
        }
        _infoUI.transform.localPosition = Vector3.zero;
        _infoUI.transform.localScale = new Vector3(1, 1, 1);

        Vector2 ScreenPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_target, Input.mousePosition, _uiCamera, out ScreenPoint);
        _infoUI.localPosition = ScreenPoint;

        float uiPos = _infoUI.localPosition.x + _infoUI.rect.width;
        if (Screen.width / 2 <= uiPos)
            _infoUI.localPosition = new Vector2(_infoUI.localPosition.x - _infoUI.rect.width, _infoUI.localPosition.y);


        var infoText = _infoUI.GetComponentInChildren<Text>().text;

        switch (item._item.Type)
        {
            case ItemManager.ITEM_TYPE.CHARACTER_EXP_POSION:
                infoText = "경험치 포션\n\n";
                break;

            case ItemManager.ITEM_TYPE.EQUIPMENT_SMELTING:
                infoText = "장비 강화석\n\n";
                break;
        }

        switch (item._item.Rarity)
        {
            case ItemManager.ITEM_RARITY.COMMON:
                infoText += "Common\n\n";
                break;

            case ItemManager.ITEM_RARITY.RARE:
                infoText += "Rare\n\n";
                break;
        }


        infoText += item._item.Value.ToString() + "\n\n";

        infoText += item._numOfItem.ToString() + " 개 남음";
        _infoUI.GetComponentInChildren<Text>().text = infoText;
    }

    public void SetTargetCanvasRectTransform(LobyManager.SCENE_STATE scene)
    {
        switch (scene)
        {
            case LobyManager.SCENE_STATE.INVENTORY:
                _target = _invenUICanvas.GetComponent<RectTransform>();
                break;

            case LobyManager.SCENE_STATE.SHOP:
                _target = _shopUICanvas.GetComponent<RectTransform>();
                break;
        }
    }



    public IEnumerator SetNotifyText(string text)
    {
        _notifyText.gameObject.SetActive(true);

        switch (LobyManager._instnace.CurSceneState)
        {
            case LobyManager.SCENE_STATE.LOBY:
                _notifyText.transform.parent = _lobyUICanvas.transform;
                break;


            case LobyManager.SCENE_STATE.CHARACTER_LIST:
                _notifyText.transform.parent = _charactersUICanvas.transform;
                break;


            case LobyManager.SCENE_STATE.INVENTORY:
                _notifyText.transform.parent = _invenUICanvas.transform;
                break;


            case LobyManager.SCENE_STATE.SHOP:
                _notifyText.transform.parent = _shopUICanvas.transform;
                break;


            case LobyManager.SCENE_STATE.EQUIPMENT_SMELT:
                _notifyText.transform.parent = _equipmentSmeltUI.transform;
                break;
        }

        _notifyText.transform.localPosition = Vector3.zero;
        _notifyText.transform.localScale = new Vector3(1, 1, 1);
        _notifyText.text = text;

        yield return new WaitForSeconds(1.5f);

        _notifyText.gameObject.SetActive(false);
    }
}
