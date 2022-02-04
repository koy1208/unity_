using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobyManager : MonoBehaviour
{
    public static LobyManager _instnace = null;

    public enum SCENE_STATE
    {
        LOBY,
        CHARACTER_LIST,
        INVENTORY,
        SHOP,
        EQUIPMENT_SMELT
    }

    SCENE_STATE _curSceneState = SCENE_STATE.LOBY;
    public SCENE_STATE CurSceneState { get { return _curSceneState; } }

    bool isInit = false;

    //
 

    private void Awake()
    {
       if(_instnace ==null)
        {
            _instnace = this;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        isInit = false;
        this.InitLoby();
    }

    // Update is called once per frame
   
    public void SceneStateChange(SCENE_STATE state)
    {
        _curSceneState = state;
        this.InitLoby();
        LobySceneUIManager._instance.SetTargetCanvasRectTransform(state);
    }

    private void InitLoby()
    {
        switch(_curSceneState)
        {
            case SCENE_STATE.LOBY:
                LobySceneUIManager._instance.LobyUIRender();
                break;

            case SCENE_STATE.CHARACTER_LIST:
                LobySceneUIManager._instance.CharacterListUIRender();
                break;

            case SCENE_STATE.INVENTORY:
                LobySceneUIManager._instance.InventoryUIRender();
                break;

            case SCENE_STATE.SHOP:
                LobySceneUIManager._instance.ShopUIRender();
                break;

            case SCENE_STATE.EQUIPMENT_SMELT:
                LobySceneUIManager._instance.EquipmentSmeltUIRender();
                break;
        }
    }


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            switch (_curSceneState)
            {
                case SCENE_STATE.LOBY:
                    break;

                case SCENE_STATE.CHARACTER_LIST:
            
                    break;

                case SCENE_STATE.INVENTORY:
                    this.LeftMouseClick_Event_ShowItemInfo();
                    break;


                case SCENE_STATE.SHOP:
                    this.LeftMouseClick_Event_ShowItemInfo();
                    break;

            }
        }
        else
        {

            switch (_curSceneState)
            {
                case SCENE_STATE.LOBY:
                    break;

                case SCENE_STATE.CHARACTER_LIST:

                    break;

                case SCENE_STATE.INVENTORY:
                    LobySceneUIManager._instance.ResetLeftClickEvent();
                    break;


                case SCENE_STATE.SHOP:
                    LobySceneUIManager._instance.ResetLeftClickEvent();
                    break;
            }
        }
    }
    
    private void LeftMouseClick_Event_ShowItemInfo()
    {
        var curObject = EventSystem.current.currentSelectedGameObject;
        //var hitObject = GameManager._instance.GetHitByTouchedMouse();

        if (curObject == null)
            return;
        string objectName = curObject.transform.name;

        string[] objectStr = objectName.Split('_');

        if (objectStr.Length > 1)
        {
            switch (objectStr[0])
            {
                case "Equipment":
                    LobySceneUIManager._instance.ShowEquipmentInfo(objectStr[1], _curSceneState);
                    break;

                case "Item":
                    string assetName = objectStr[1] + "_" + objectStr[2];

                    LobySceneUIManager._instance.ShowItemInfo(assetName, _curSceneState);
                    break;
            }
        }
    }
    
}
