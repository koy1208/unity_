using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character_Base : MonoBehaviour
{
    public enum CHARACTER_OWNER
    {
        PLAYER,
        ENEMY
    }

    // [SerializeField]
    Canvas _canvas;
    public Canvas characterCanvas { get { return _canvas; } }

    Slider _hpBar;
    Transform _buffImageTrans;

    Image _buffImageBase;
    Text _buffCountText;


    int _numOfSkill;
    public int SkillNum { get { return _numOfSkill; } }


    CHARACTER_OWNER _owner;
    public CHARACTER_OWNER Owner { get { return _owner; } }

    bool _isDead = false;

    float _maxHP;
    public float MaxHP { get { return _maxHP; } set { _maxHP = value; } }

    float _Hp;
    public float HP { get { return _Hp; } set { _Hp = value; } }

    float _Atk;
    public float ATK { get { return _Atk; } set { _Atk = value; } }

    float _Def;
    public float DEF { get { return _Def; } set { _Def = value; } }

    float _criticalDamage;
    public float CriticalDamage { get { return _criticalDamage; } set { _criticalDamage = value; } }

    float _criticalPercentage;
    public float CriticalPercentage { get { return _criticalPercentage; } set { _criticalPercentage = value; } }

    float _speed;
    public float Speed { get { return _speed; } set { _speed = value; } }

    List<string> _skillList;
    public List<string> SkillList { get { return _skillList; } }

    
    Character_SO _characterState;
    public Character_SO Character_so { get { return _characterState; } }

    //Level, Exp 값이 들어 있는 클래스 받아오기
    private CharacterManager.CharacterInfo _characterInfo;
    public CharacterManager.CharacterInfo CharacterInfo { get { return _characterInfo; } }

    Animator _animator;

    public void setInfo(CharacterManager.CharacterInfo info)
    {
        _characterInfo = info;
    }



    public class BuffInfo
    {
        Image _buffImage;
        public Image BuffImage { get { return _buffImage; } set { _buffImage = value; } }

        BuffManager.BUFF _buff;
        public BuffManager.BUFF Buff { get { return _buff; } }

        int _buffCount;
        public int BuffCount { get { return _buffCount; } set { _buffCount = value; } }

        bool _isEveryTurnCheck;
        public bool IsEveryTurnCheck { get { return _isEveryTurnCheck; } }

        bool _areadyChecked;
        public bool AreadyChecked { get { return _areadyChecked; } set { _areadyChecked = value; } }


        public BuffInfo(BuffManager.BUFF buff, int buffCount, bool isEveryTurnCheck)
        {
            _buff = buff;
            _buffCount = buffCount;
            _isEveryTurnCheck = isEveryTurnCheck;
            _areadyChecked = false;
        }

        public void BuffCountDown()
        {
            _buffCount--;
        }
    }

    List<BuffInfo> _buffList;
    public List<BuffInfo> BuffList { get { return _buffList; } }
    //List<스킬스프라이트> _skiilList;

    // List<string> SkillTypeNames;

    GameObject _target;
    public GameObject Target { get { return _target; } }


    private void Awake()
    {
        _skillList = new List<string>();
        _buffList = new List<BuffInfo>();

        _animator = this.GetComponent<Animator>();
    }

    public void RefreshStatByLevelUp()
    {
        if (_characterState == null)
            return;

        _maxHP += _characterState._maxHPbyLevel;
        _Atk += _characterState._AtkbyLevel;
        _Def += _characterState._DefbyLevel;
    }

    public void CharacterInit(float maxHP, float atk, float def, float criDamage, float criPercent, float speed, CHARACTER_OWNER owner)
    {
        this._maxHP = maxHP;
        this._Hp = this._maxHP;
        this._Atk = atk;
        this._Def = def;
        this._criticalDamage = criDamage;
        this._criticalPercentage = criPercent;
        this._speed = speed;
        this._owner = owner;
    }

    public void CharacterInit(float maxHP, float atk, float def, float criDamage, float criPercent, float speed)
    {
        this._maxHP = maxHP;
        this._Hp = this._maxHP;
        this._Atk = atk;
        this._Def = def;
        this._criticalDamage = criDamage;
        this._criticalPercentage = criPercent;
        this._speed = speed;
    }


    public void CharacterInit(Character_SO asset, CHARACTER_OWNER onwer)
    {
        if (asset == null)
            return;

        if (_characterInfo != null)
        {
            this._maxHP = asset._maxHP + asset._maxHPbyLevel * (_characterInfo.Level - 1);
            this._Atk = asset._Atk + asset._AtkbyLevel * (_characterInfo.Level - 1);
            this._Def = asset._Def + asset._DefbyLevel * (_characterInfo.Level - 1);
        }
        else
        {
            //레벨을 받는 식으로 함수 더 만들기 적 오브젝트용
            this._maxHP = asset._maxHP;
            this._Atk = asset._Atk;
            this._Def = asset._Def;
        }
        this._Hp = this._maxHP;
        this._criticalDamage = asset._criticalDamage;
        this._criticalPercentage = asset._criticalPercentage;
        this._speed = asset._speed;
        this._owner = onwer;

        for (int i = 0; i < asset._skillScriptNames.Length; i++)
        {
            _skillList.Add(asset._skillScriptNames[i]);
            _numOfSkill++;
        }

        if (_characterInfo != null)
            if (_characterInfo.Equipments != null)
                this.CharacterEquipmentInit();

        _characterState = asset;
    }

    
    private void CharacterEquipmentInit()
    {
        var EquipmentIDs = _characterInfo.Equipments;
        Equipment_Base.Equipment_Info[] equipment = new Equipment_Base.Equipment_Info[(int)EquipmentManager.EquipmentParts.PARTS_END];


        for(int i =0; i < EquipmentIDs.Length; i++)
        {
            if (EquipmentIDs[i] == -1)
                continue;

            equipment[i] = Player._instance.FindEquipmentInHasEquipment(EquipmentIDs[i]);
        }
       // Equipment_Base.Equipment_Info[] equipment = _characterInfo.Equipments;

        for (int i = 0; i < (int)EquipmentManager.EquipmentParts.PARTS_END; i++)
        {
            if (equipment[i] == null)
                continue;

            EquipmentManager.EquipmentStatValue[] statVal = equipment[i].StatValues;

            for (int j = 0; j < statVal.Length; j++)
            {
                if (statVal[j] == null)
                    continue;

              
                float val = statVal[j].ReturnApplyValue(this.gameObject);

                switch (statVal[j].Stat)
                {
                    case EquipmentManager.EquipmentStat.HP:
                        _maxHP += val;
                        _maxHP = Mathf.Round(_maxHP);
                        break;

                    case EquipmentManager.EquipmentStat.ATK:
                        _Atk += val;
                        _Atk = Mathf.Round(_Atk);
                        break;

                    case EquipmentManager.EquipmentStat.DEF:
                        _Def += val;
                        _Def = Mathf.Round(_Def);
                        break;

                    case EquipmentManager.EquipmentStat.CRIDMG:
                        _criticalDamage += val;
                        break;

                    case EquipmentManager.EquipmentStat.CRIPER:
                        _criticalPercentage += val;
                        break;

                    case EquipmentManager.EquipmentStat.SPEED:
                        Speed += val;
                        break;
                }
            }
        }
    }
    

    /// <summary>
    /// 전투 UI
    /// </summary>
    public void UI_Init()
    {
        GameObject can = (GameObject)Resources.Load("Prefabs/Character_Canvas");

        _canvas = Instantiate(can.GetComponent<Canvas>());
        _canvas.transform.parent = this.gameObject.transform;
        _canvas.transform.forward = Camera.main.transform.forward;

        _canvas.transform.position = this.gameObject.transform.position;
        //_numOfSkill = 1;//_skillList.Count;

        GameObject hpBar = (GameObject)Resources.Load("Prefabs/Character_HP_Bar");
        _hpBar = Instantiate(hpBar.GetComponent<Slider>());
        _hpBar.transform.parent = _canvas.transform;
        _hpBar.transform.forward = _canvas.transform.forward;
        _hpBar.transform.localPosition = Vector3.zero;

        _hpBar.transform.position += new Vector3(0, 1.5f, 0);

        //Debug.Log(_Hp / _maxHP);
        _Hp = _maxHP;
        _hpBar.value = Mathf.Clamp01(_Hp / _maxHP);

        
        var childsTrans = _hpBar.GetComponentsInChildren<Transform>();

        for (int i = 0; i < childsTrans.Length; i++)
        {
            if (childsTrans[i].gameObject.name == "BuffImageTransfrom")
            {
                _buffImageTrans = childsTrans[i];
                break;
            }
        }

        GameObject BuffIamge = (GameObject)Resources.Load("Prefabs/BuffImage");
        _buffImageBase = BuffIamge.GetComponent<Image>();
        _buffCountText = _buffImageBase.GetComponentInChildren<Text>();
    }

    public void BuffAndDeBuffCheck()
    {
        if (_buffList.Count > 0)
        {
            //_buffList[0].GetType();
            for (int cnt = 0; cnt < _buffList.Count; cnt++)
            {
                if (_buffList[cnt].IsEveryTurnCheck == false && _buffList[cnt].AreadyChecked == true)
                    continue;

                var buff = _buffList[cnt].Buff;
                BuffManager._instance.BuffEffect(buff, this.gameObject);

                if (_buffList[cnt].IsEveryTurnCheck == false)
                    _buffList[cnt].AreadyChecked = true;

            }
        }
    }

    private void BuffImageRefresh(int num, BuffInfo buffInfo)
    {
        float x;
        float y;

        x = num % 4;
        y = num / 4;

        var childsTrans = _hpBar.GetComponentsInChildren<Transform>();
        Transform buffImageTrans;

        for (int i = 0; i < childsTrans.Length; i++)
        {
            if (childsTrans[i].gameObject.name == "BuffImageTransfrom")
            {
                buffImageTrans = childsTrans[i];

                var BuffImage = Instantiate(_buffImageBase);
                BuffImage.transform.SetParent(buffImageTrans);
                BuffImage.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);
                BuffImage.transform.localPosition = new Vector3(16 + (9 * 16 * (x - 1)), 16 * 10  * y, 0.0f);
                BuffImage.sprite = BuffManager._instance.GetBuffSprite(buffInfo.Buff);
                var BuffText = BuffImage.GetComponentInChildren<Text>();
                BuffText.text = buffInfo.BuffCount.ToString();

                buffInfo.BuffImage = BuffImage;
                break;
            }
        }
    }

    
    public Skill_Interface UseSkiil(int index)
    {
        var skill = (Skill_Interface)this.GetComponent(System.Type.GetType(_skillList[index]));
        skill.UseSkill(this.gameObject, _target);

        return skill;
    }
    
    public SkillManager.SKILL_TARGET GetSkillTarget(int index)
    {
        var skill = (Skill_Interface)this.GetComponent(System.Type.GetType(_skillList[index]));

        return skill.Skill_Target;
    }

    public void BuffAndDeBuffCountDown()
    {

        if (_buffList.Count > 0)
        {
            for (int cnt = 0; cnt < _buffList.Count; cnt++)
            {
                Debug.Log(this.gameObject.name + ": BuffCount(" + _buffList[cnt].BuffCount + ")");

                _buffList[cnt].BuffCountDown();
                _buffList[cnt].BuffImage.GetComponentInChildren<Text>().text = _buffList[cnt].BuffCount.ToString();

                if (_buffList[cnt].BuffCount == 0)
                {
                    BuffManager._instance.BuffEffectDelete(_buffList[cnt].Buff, this.gameObject);
                    BuffInfo delBuff = _buffList[cnt];
                    Destroy(_buffList[cnt].BuffImage.gameObject);
                    _buffList.Remove(delBuff);
                    cnt--;
                }
            }
        }
    }


    public void BuffListInit()
    {
        for(int i =0; i < _buffList.Count; i++)
        {
            BuffManager._instance.BuffEffectDelete(_buffList[i].Buff, this.gameObject);
            _buffList.Remove(_buffList[i]);

            i--;
        }
    }
    

    
    public void Damaged(float damage, float criDamage = 100, bool isCri = false, bool ignorDEF = false, bool damageByBuff = false)
    {
        if (ignorDEF == false)
            this.CalculateNewDamageByDEF(ref damage);

        if (isCri == true)
        {
            damage *= criDamage / 100.0f;
            Debug.Log("Critical");
        }

        damage = Mathf.Max(0, damage);
        damage = Mathf.Round(damage);
        _Hp -= damage;

        if(_animator != null)
        {
            _animator.SetTrigger("Damaged");
        }

        if (_Hp <= 0.0f)
        {
            _isDead = true;
            //StageManager._instance.CharacterDead(this.gameObject);


            if (_animator != null)
            {
                _animator.SetTrigger("Die");
            }
        }

        Debug.Log(this.name + "  Damaged  " + damage.ToString());

        // _canvas.transform.forward = Camera.main.transform.forward;
        _hpBar.value = Mathf.Clamp01(_Hp / _maxHP);



        StartCoroutine(StageManager._instance.ShowDamage(damage, damageByBuff, this));
    }

    public void characterRemove()
    {
        StageManager._instance.CharacterDead(this.gameObject);
    }

    


    public bool IsDead()
    {
        return _isDead;
    }


    protected void CalculateNewDamageByDEF(ref float damage)
    {
        damage -= _Def;
        Mathf.Max(1.0f, damage);
    }

    protected void AddSkill(string skillName)
    {
        if (skillName == null)
            return;

        if (_skillList.Contains(skillName))
            return;

        _skillList.Add(skillName);
        _numOfSkill = _skillList.Count;
        this.gameObject.AddComponent(System.Type.GetType(skillName));
    }

    public void AddBuff(BuffManager.BUFF buff, int buffCount, bool isEveryTurnCheck)
    {
        BuffInfo buffInfo = new BuffInfo(buff, buffCount, isEveryTurnCheck);

        for (int i = 0; i < _buffList.Count; i++)
        {
            if (_buffList[i].Buff == buff)
            {
                if (_buffList[i].BuffCount < buffCount)
                {
                    _buffList[i].BuffCount = buffCount;
                    _buffList[i].BuffImage.GetComponentInChildren<Text>().text = _buffList[i].BuffCount.ToString();
                }
                return;
            }
        }

        _buffList.Add(buffInfo);
        this.BuffImageRefresh(_buffList.Count, buffInfo);
    }

    public void setTarget(GameObject target)
    {
        _target = target;
    }
}
