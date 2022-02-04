using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    static public StageManager _instance = null;


    enum STAGE_STATE
    {
        START,
        ADD_SPEED,
        TURN_START,
        CHECK_BUFF,
        ATTACK,
        TURN_END,
        END,
        NONE
    }

    STAGE_STATE _curStageState = STAGE_STATE.START;

    List<GameObject> _totalCharacterList;

    GameObject _currTurnedCharater;
    string _curCharaterAIName;

    GameObject _targetObject = null;

    float _speedGaugeMaxValue = 1000;


    int _selectedSkillNum = -1;
    bool _isUseSkill = false;
    SkillManager.SKILL_TARGET _curSkillTarget = SkillManager.SKILL_TARGET.NONE;

    //*****************
    [SerializeField]
    GameObject _partyPosition;

    [SerializeField]
    GameObject _enemiesPosition;

    CharacterManager.CharacterInfo[] _playerParty;
    CharacterManager.CharacterInfo[] _enemyParty;

    Stage_SO _curStage;

    // UI 변수

    Vector3 _speedGaugePosition = new Vector3(-380, 50, 0);

    [SerializeField]
    Button[] _skillButton;

    [SerializeField]
    Image _speedGaugeSource;

    [SerializeField]
    Image _speedIconSource;

    [SerializeField]
    GameObject _targetingUISource;
    GameObject _targetingUI = null;

    [SerializeField]
    Text _notifyText;


    //List<GameObject> _damageTextObjectList = new List<GameObject>();

    float _speedGaugeSourceHeight;

    struct st_SpeedIcon
    {
        Image _IconImage;
        string _ownerName;
        float _speedGaugeValue;

        public Image IconImage { get { return _IconImage; } }
        public string OwnerName { get { return _ownerName; } }
        public float SpeedValue { get { return _speedGaugeValue; } }

        public st_SpeedIcon(Image iconImage, string ownerName, float value)
        {
            _IconImage = iconImage;
            _ownerName = ownerName;
            _speedGaugeValue = value;
        }
        public void AddSpeedValue(float speed)
        {
            _speedGaugeValue += speed;
        }

        public void SpeedGaugeValueReset()
        {
            _speedGaugeValue = 0.0f;
        }
    }

    List<st_SpeedIcon> _speedIconList;

    [SerializeField]
    Text _damageText;

    List<Text> _damageTextList;

  

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test");

        _totalCharacterList = new List<GameObject>();
        _speedIconList = new List<st_SpeedIcon>();

        _damageTextList = new List<Text>();

        _playerParty = new CharacterManager.CharacterInfo[4];
        _enemyParty = new CharacterManager.CharacterInfo[4];

        if (_targetingUI == null)
        {
            _targetingUI = Instantiate(_targetingUISource);
            _targetingUI.name = "TargetingUI";
        }

        this.StageStart();

    }

    // Update is called once per frame
    void Update()
    {

        switch (_curStageState)
        {
            case STAGE_STATE.START:
                // this.StageStart();
                break;

            case STAGE_STATE.ADD_SPEED:
                this.AddSpeed();
                break;

            case STAGE_STATE.TURN_START:
                this.TurnStart();
                break;

            case STAGE_STATE.CHECK_BUFF:
                this.CheckBuff();
                break;

            case STAGE_STATE.ATTACK:
                this.Attack();
                break;

            case STAGE_STATE.TURN_END:
                this.TurnEnd();
                break;

            case STAGE_STATE.END:
                this.StageEnd();
                break;
        }
    }


    //partyPos (1 ~ 4) 위치값
    //pratyPos (5) forward값


    private void StageStart()
    {
        Stage_SO stageInfo;
        string path = "Assets/Resources/ScriptableObject/Stage_ScriptableObject/"
                + GameManager._instance.StageName + "_SO.asset";
        stageInfo = (Stage_SO)ScriptableObject.CreateInstance(typeof(Stage_SO));
        stageInfo = AssetDatabase.LoadAssetAtPath<Stage_SO>(path);
        stageInfo.StageInit();
        _enemyParty = stageInfo.EnemyParty;

        //_playerParty = 

        int[] partyID = Player._instance.PlayerInfo.InPartyCharacterArray;

        for(int i =0; i < _playerParty.Length; i++)
        {
            if (partyID[i] == -1)
                continue;

            _playerParty[i] = Player._instance.PlayerInfo.FindCharacterAtID(partyID[i]);
        }

        Transform[] partyPos = _partyPosition.GetComponentsInChildren<Transform>();
        Transform[] enemiesPos = _enemiesPosition.GetComponentsInChildren<Transform>();

        for (int i = 0; i < _playerParty.Length; i++)
        {
            if (_playerParty[i] == null)
                continue;

            if (_playerParty[i].CharacterName == "" || _playerParty[i].CharacterName == null)
                continue;




            Character_SO asset;
            string assetPath = "Assets/Resources/ScriptableObject/Character_ScriptableObject/"
                + _playerParty[i].CharacterName + "_SO.asset";

            asset = (Character_SO)ScriptableObject.CreateInstance(typeof(Character_SO));
            asset = AssetDatabase.LoadAssetAtPath<Character_SO>(assetPath);
            asset.SourceRender();

            GameObject copy = asset.CharacterSource;
            _totalCharacterList.Add(copy);

            Character_Base copyBase = copy.GetComponent<Character_Base>();
            copyBase.setInfo(_playerParty[i]);
            copyBase.CharacterInit(asset, Character_Base.CHARACTER_OWNER.PLAYER);

            copy.transform.position = partyPos[i + 1].position;
            copy.transform.position += new Vector3(0.0f, 1.0f, 0.0f);

            copy.transform.forward = partyPos[i + 1].transform.forward;
        }

        GameObject[] EnemyParty = new GameObject[4];

        for (int i = 0; i < _enemyParty.Length; i++)
        {
            if (_enemyParty[i] == null)
                continue;

            var Enemy = EnemyInit(_enemyParty[i]);
            Enemy.transform.position = enemiesPos[i + 1].position;
            Enemy.transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            Enemy.transform.forward = enemiesPos[i + 1].transform.forward;//new Vector3(5.0f, 0.0f, 1.0f);
            EnemyParty[i] = Enemy;
        }

        stageInfo.StageEnemyInit(ref EnemyParty);

        this.InitStageUI();

        _curStageState = STAGE_STATE.ADD_SPEED;
        _curStage = stageInfo;
        return;
    }

    private GameObject EnemyInit(CharacterManager.CharacterInfo enemyInfo)
    {
        Character_SO EnemyAsset;
        string EassetPath = "Assets/Resources/ScriptableObject/Character_ScriptableObject/" + enemyInfo.CharacterName + "_SO.asset";
        EnemyAsset = (Character_SO)ScriptableObject.CreateInstance(typeof(Character_SO));
        EnemyAsset = AssetDatabase.LoadAssetAtPath<Character_SO>(EassetPath);
        EnemyAsset.SourceRender();

        ///수정
        GameObject Ecopy = EnemyAsset.CharacterSource;
        _totalCharacterList.Add(Ecopy);

        Character_Base EcopyBase = Ecopy.GetComponent<Character_Base>();
        EcopyBase.setInfo(enemyInfo);
        EcopyBase.CharacterInit(EnemyAsset, Character_Base.CHARACTER_OWNER.ENEMY);

        return Ecopy;

        //Ecopy.transform.position += new Vector3(5.0f, 0.0f, 1.0f);
    }

    private void InitStageUI()
    {
        GameObject stage_ui = GameObject.Find("Stage_UI");
        // GameObject speedGauge = Instantiate(c).gameObject;
        // speedGauge.transform.SetParent(stage_ui.transform);
        // speedGauge.transform.localPosition = _speedGaugePosition;
        _speedGaugeSourceHeight = _speedGaugeSource.GetComponent<Image>().rectTransform.rect.height;

        foreach (GameObject character in _totalCharacterList)
        {
            Character_Base CB = character.GetComponent<Character_Base>();

            float characterSpeed = CB.Speed;
            Image copyIcon = Instantiate(_speedIconSource);
            copyIcon.transform.SetParent(_speedGaugeSource.transform);
            copyIcon.transform.localPosition = new Vector3(0, _speedGaugeSourceHeight / 2.0f - characterSpeed * 2.0f, 0);

            //수정할 부분

            if (CB.CharacterInfo.Overlap != 0)
            {
                copyIcon.name = CB.CharacterInfo.CharacterName + "_" + CB.CharacterInfo.Overlap.ToString();
            }
            else
                copyIcon.name = CB.CharacterInfo.CharacterName;

            if (CB.Owner == Character_Base.CHARACTER_OWNER.ENEMY)
                copyIcon.name += "_NPC";

            character.name = copyIcon.name;

            st_SpeedIcon SI = new st_SpeedIcon(copyIcon, character.name, characterSpeed * 2.0f);
            copyIcon.name += "_SpeedIcon";

            _speedIconList.Add(SI);

            CB.UI_Init();


        }
    }

    private void AddSpeed()
    {
        if (_currTurnedCharater != null)
        {
            _curStageState = STAGE_STATE.TURN_START;
            return;
        }
        else
        {
            for (int i = 0; i < _totalCharacterList.Count; i++)
            {
                if (_totalCharacterList[i].GetComponent<Character_Base>().IsDead())
                    continue;

                for (int j = 0; j < _speedIconList.Count; j++)
                {
                    var CB = _totalCharacterList[i].GetComponent<Character_Base>();
                    string CharacterName;

                    if (CB.CharacterInfo.Overlap != 0)
                    {
                        CharacterName = CB.CharacterInfo.CharacterName + "_" + CB.CharacterInfo.Overlap.ToString();
                    }
                    else
                        CharacterName = CB.CharacterInfo.CharacterName;

                    if (CB.Owner == Character_Base.CHARACTER_OWNER.ENEMY)
                        CharacterName += "_NPC";

                    if (CharacterName != _speedIconList[j].OwnerName)
                        continue;

                    float speed = _totalCharacterList[i].GetComponent<Character_Base>().Speed;


                    //***************************************************//
                    st_SpeedIcon copyIcon = _speedIconList[j];
                    copyIcon.AddSpeedValue(speed);


                    float speedIconMoveValueInGaugeSource = _speedGaugeSourceHeight / 2.0f - _speedGaugeSourceHeight * Mathf.Clamp01(copyIcon.SpeedValue / _speedGaugeMaxValue);

                    Transform iconTrans = copyIcon.IconImage.transform;
                    iconTrans.localPosition = new Vector3(0, speedIconMoveValueInGaugeSource, 0);

                    if (copyIcon.SpeedValue >= _speedGaugeMaxValue && _currTurnedCharater == null)
                    {
                        this.ChangeCurCharacter(_totalCharacterList[i]);

                        ////////////////////////////////////
                        ///
                        this.ChangeUI();
                        _curStageState = STAGE_STATE.TURN_START;

                    }

                    _speedIconList[j] = copyIcon;
                }
            }
        }
    }

    private void TurnStart()
    {
        _curStageState = STAGE_STATE.CHECK_BUFF;
    }

    private void CheckBuff()
    {

        _currTurnedCharater.GetComponent<Character_Base>().BuffAndDeBuffCheck();
        _curStageState = STAGE_STATE.ATTACK;

        //스킬 선택 후에 호출하도록 변경
       // this.SetTarget();
    }


    private void Attack()
    {
        Character_Base CB = _currTurnedCharater.GetComponent<Character_Base>();

        Debug.Log(_currTurnedCharater.name + ": ATK(" + CB.ATK + ")");

        if (CB.Owner == Character_Base.CHARACTER_OWNER.PLAYER)
        {

            this.SelectTarget();
            return;
        }


        for (int i = 0; i < CB.SkillList.Count; i++)
        {

            // 스킬AI 우선순위 추가
            if (CB.SkillList[i] != null)
            {
                switch (i)
                {
                    case 0:
                        this.OnClickSkillButton1();
                        break;

                    case 1:
                        this.OnClickSkillButton2();
                        break;

                    case 2:
                        this.OnClickSkillButton3();
                        break;

                    case 3:
                        this.OnClickSkillButton4();
                        break;
                }
            }
        }

    }

    //스킬 선택 후에 호출하도록 변경
    //************************************************************************

    //전 캐릭터 스킬 타켓 자동 선택
    private void SetTarget()
    {
        Character_Base.CHARACTER_OWNER CurCharacterOwner = (_currTurnedCharater.GetComponent<Character_Base>()).Owner;
        for (int i = 0; i < _totalCharacterList.Count; i++)
        {
            if (_totalCharacterList[i].GetComponent<Character_Base>().IsDead())
                continue;

            Character_Base.CHARACTER_OWNER CharacterOwner = (_totalCharacterList[i].GetComponent<Character_Base>().Owner);//(_totalCharacterList[i].name + "_AI")).Owner;


            if (CurCharacterOwner != CharacterOwner)
            {
                _currTurnedCharater.GetComponent<Character_Base>().setTarget(_totalCharacterList[i]);
                _targetObject = _totalCharacterList[i];
                return;
            }
        }
    }

    //플레이어 스킬 타겟 선택
    private void SelectTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit ray;
            ray = GameManager._instance.GetHitByTouchedMouse();

            Character_Base character = ray.transform.GetComponent<Character_Base>();
            if (character != null)
            {
                switch(_curSkillTarget)
                {
                    case SkillManager.SKILL_TARGET.ENEMY_ALL:
                    case SkillManager.SKILL_TARGET.ENEMY_ONE:
                        if(character.Owner == Character_Base.CHARACTER_OWNER.PLAYER)
                        {
                            StartCoroutine(this.ShowNotify("잘못된 대상입니다"));
                            return;
                        }

                        break;

                    case SkillManager.SKILL_TARGET.TEAM_ALL:
                    case SkillManager.SKILL_TARGET.TEAM_ONE:
                        if (character.Owner == Character_Base.CHARACTER_OWNER.ENEMY)
                        {
                            StartCoroutine(this.ShowNotify("잘못된 대상입니다"));
                            return;
                        }
                        break;
                }

                _targetingUI.SetActive(true);

                _currTurnedCharater.GetComponent<Character_Base>().setTarget(ray.transform.gameObject);
                _targetObject = ray.transform.gameObject;
                _targetingUI.transform.parent = _targetObject.transform;
                _targetingUI.transform.localPosition = Vector3.zero;
                _targetingUI.transform.localScale = new Vector3(2, 2, 2);
                _targetingUI.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));


                Debug.Log("targeting: " + _targetObject.name);
            }
        }
    }
    //************************************************************************

    private IEnumerator ShowNotify(string text)
    {
        _notifyText.gameObject.SetActive(true);
        _notifyText.text = text;

        yield return new WaitForSeconds(2.0f);

        _notifyText.gameObject.SetActive(false);

    }

    public IEnumerator ShowDamage(float damage, bool damageByBuff = false, Character_Base cb = null)
    {

        Text damageText = Instantiate(_damageText);
        GameObject stage_ui = GameObject.Find("Stage_UI");

        Character_Base CB;

        if (damageByBuff)
        {
            CB = _currTurnedCharater.GetComponent<Character_Base>();
            damageText.transform.parent = CB.characterCanvas.transform;
            damageText.transform.forward = Camera.main.transform.forward;
            damageText.transform.position = _currTurnedCharater.transform.position + new Vector3(0, 2, 0);
            damageText.text = damage.ToString();
        }
        else
        {

            //if (_targetObject == null)
            //    _targetObject = _currTurnedCharater.GetComponent<Character_Base>().Target;

            //CB = _targetObject.GetComponent<Character_Base>();
            
            damageText.transform.parent = cb.characterCanvas.transform;
            damageText.transform.forward = Camera.main.transform.forward;
            damageText.transform.position = cb.gameObject.transform.position + new Vector3(0, 2, 0);
            damageText.text = damage.ToString();
        }


        _damageTextList.Add(damageText);


        yield return new WaitForSeconds(0.5f);

        Text removeText = _damageTextList[0];
        _damageTextList.Remove(removeText);
        Destroy(removeText.gameObject);
    }


    public void StageStateTurnEnd()
    {
        _curStageState = STAGE_STATE.TURN_END;
    }

    private void TurnEnd()
    {
        //if (_currTurnedCharater != null)
        //{
        for (int i = 0; i < _speedIconList.Count; i++)
        {
            if (_speedIconList[i].OwnerName == _currTurnedCharater.name)
            {
                st_SpeedIcon copy = _speedIconList[i];
                copy.SpeedGaugeValueReset();
                _speedIconList[i] = copy;

                this.BuffCountDown();

                if(_currTurnedCharater.GetComponent<Character_Base>().Owner == Character_Base.CHARACTER_OWNER.PLAYER)
                    this.ResetButtonUI();

                _currTurnedCharater.GetComponent<Character_Base>().setTarget(null);
                _currTurnedCharater = null;
                _targetingUI.SetActive(false);
                _targetObject = null;

                break;
            }

        }
        // }

        if (this.CheckGameOver())
        {
            _curStageState = STAGE_STATE.END;
            return;
        }

        _targetObject = null;
        _selectedSkillNum = -1;
        _curStageState = STAGE_STATE.ADD_SPEED;
        _isUseSkill = false;
    }

    private bool CheckGameOver()
    {
        int PlayerParty = 0;
        int EnemyParty = 0;

        for (int i = 0; i < _totalCharacterList.Count; i++)
        {
            Character_Base CB = _totalCharacterList[i].GetComponent<Character_Base>();

            if (CB.IsDead() == false)
            {
                if (CB.Owner == Character_Base.CHARACTER_OWNER.PLAYER)
                    PlayerParty++;
                else
                    EnemyParty++;
            }
        }

        //추가 필요
        if (EnemyParty == 0)
        {
            _curStage.ClrearStage();
            ////for (int i = 0; i < 4; i++)
            //{
            //    if (_playerParty[i] == null)
            //        continue;

            //    //_playerParty[i].TestLevelUp();
            //}
            //PlayerManager._instanse.UpdateCharacterInfo(_playerParty);
            return true;
        }
        else if (PlayerParty == 0)
        {
            return true;
        }

        return false;
    }

    private void PlayerPartyInit()
    {
        for (int i = 0; i < _totalCharacterList.Count; i++)
        {
            Character_Base CB = _totalCharacterList[i].GetComponent<Character_Base>();
            CB.BuffListInit();
            CB.HP = CB.MaxHP;
        }
    }

    private void BuffCountDown()
    {
        _currTurnedCharater.GetComponent<Character_Base>().BuffAndDeBuffCountDown();
    }

    private void StageEnd()
    {
        JsonSingleton._instance.FIleSave(Player._instance.PlayerInfo);
        Stage_UI._instance.SetStageEndUI();

        _curStageState = STAGE_STATE.NONE;   
    }

    public void ChangeCurCharacter(GameObject character)
    {
        _currTurnedCharater = character;

        Animator animator = _currTurnedCharater.GetComponent<Animator>();
        if (animator == null)
            return;

        animator.SetTrigger("Ready");
    }




    void ChangeUI()
    {
        if (_currTurnedCharater != null && _currTurnedCharater.GetComponent<Character_Base>().Owner == Character_Base.CHARACTER_OWNER.PLAYER)
        {
            var CB = _currTurnedCharater.GetComponent<Character_Base>();
            for (int cnt = 0; cnt < CB.SkillNum; cnt++)
            {
                string skillName = CB.SkillList[cnt];
                var skill = (Skill_Interface)(_currTurnedCharater.GetComponent(System.Type.GetType(skillName)));
                _skillButton[cnt].gameObject.SetActive(true);
                _skillButton[cnt].GetComponentInChildren<Text>().text = skill.SkillName;
            }
        }
    }

    void ResetButtonUI()
    {
        for (int i = 0; i < _skillButton.Length; i++)
        {
            Image[] images = null;
            images = _skillButton[_selectedSkillNum].GetComponentsInChildren<Image>();
            images[1].enabled = false;
            _skillButton[i].gameObject.SetActive(false);
        }
    }

    public void CharacterDead(GameObject character)
    {
        character.SetActive(false);
        for (int i = 0; i < _speedIconList.Count; i++)
        {


            if (character.name == _speedIconList[i].OwnerName)
            {
                _speedIconList[i].IconImage.gameObject.SetActive(false);
                return;
            }
        }
    }

    public void OnClickSkillButton1()
    {
        if (_isUseSkill)
            return;

        if (_selectedSkillNum != 0 && _currTurnedCharater.GetComponent<Character_Base>().Owner == Character_Base.CHARACTER_OWNER.PLAYER)
        {
            Image[] images = null;
            if (_selectedSkillNum != -1)
            {
                images = _skillButton[_selectedSkillNum].GetComponentsInChildren<Image>();
                images[1].enabled = false;
                _targetingUI.SetActive(false);
                _targetObject = null;

                _currTurnedCharater.GetComponent<Character_Base>().setTarget(null);
            }

            _selectedSkillNum = 0;
            _curSkillTarget = _currTurnedCharater.GetComponent<Character_Base>().GetSkillTarget(0);

            images = _skillButton[0].GetComponentsInChildren<Image>();
            images[1].enabled = true;
        }
        else
        {
            _currTurnedCharater.GetComponent<Character_Base>().UseSkiil(0);
            _isUseSkill = true;
            //_curStageState = STAGE_STATE.TURN_END;
        }


    }

    public void OnClickSkillButton2()
    {
        if (_isUseSkill)
            return;

        if (_selectedSkillNum != 1 && _currTurnedCharater.GetComponent<Character_Base>().Owner == Character_Base.CHARACTER_OWNER.PLAYER)
        {
            Image[] images = null;
            if (_selectedSkillNum != -1)
            {
                images = _skillButton[_selectedSkillNum].GetComponentsInChildren<Image>();
                images[1].enabled = false;
                _targetingUI.SetActive(false);
                _targetObject = null;
            }


            _selectedSkillNum = 1;
            _curSkillTarget = _currTurnedCharater.GetComponent<Character_Base>().GetSkillTarget(1);

             images =_skillButton[1].GetComponentsInChildren<Image>();
            images[1].enabled = true;

        }
        else
        {
            _currTurnedCharater.GetComponent<Character_Base>().UseSkiil(1);
            //_curStageState = STAGE_STATE.TURN_END;
            _isUseSkill = true;
        }

    }

    public void OnClickSkillButton3()
    {
        if (_isUseSkill)
            return;

        if (_selectedSkillNum != 2 && _currTurnedCharater.GetComponent<Character_Base>().Owner == Character_Base.CHARACTER_OWNER.PLAYER)
        {
            Image[] images = null;
            if (_selectedSkillNum != -1)
            {
                images = _skillButton[_selectedSkillNum].GetComponentsInChildren<Image>();
                images[1].enabled = false;
                _targetingUI.SetActive(false);
                _targetObject = null;
            }

            _selectedSkillNum = 2;
            _curSkillTarget = _currTurnedCharater.GetComponent<Character_Base>().GetSkillTarget(2);

            images = _skillButton[2].GetComponentsInChildren<Image>();
            images[1].enabled = true;
        }
        else
        {
            _currTurnedCharater.GetComponent<Character_Base>().UseSkiil(2);
            //_curStageState = STAGE_STATE.TURN_END;
            _isUseSkill = true;
        }
    }


    //

    public void OnClickSkillButton4()
    {
        if (_isUseSkill)
            return;

        if (_selectedSkillNum != 3)
        {
            _selectedSkillNum = 3;
        }
        else
        {
            _currTurnedCharater.GetComponent<Character_Base>().UseSkiil(3);
            _curStageState = STAGE_STATE.TURN_END;
            _isUseSkill = true;
        }

    }


    public GameObject[] GetPlayerParty()
    {
        GameObject[] playerparty = new GameObject[4];

        int cnt = 0;
        for (int i = 0; i < _totalCharacterList.Count; i++)
        {
            if (_totalCharacterList[i].GetComponent<Character_Base>().Owner == Character_Base.CHARACTER_OWNER.PLAYER)
            {
                playerparty[cnt] = _totalCharacterList[i];
                cnt++;
            }
        }

        return playerparty; 
    }

    public GameObject[] GetEnemyParty()
    {
        GameObject[] enemyParty = new GameObject[4];

        int cnt = 0;
        for (int i = 0; i < _totalCharacterList.Count; i++)
        {
            if (_totalCharacterList[i].GetComponent<Character_Base>().Owner == Character_Base.CHARACTER_OWNER.ENEMY)
            {
                enemyParty[cnt] = _totalCharacterList[i];
                cnt++;
            }
        }

        return enemyParty;
    }

    //삭제
    public void OnClickTestInputCurTurnedCharacter()
    {
        this.StageStart();
    }

}
