using UnityEngine;

[CreateAssetMenu(fileName = "Character_Base_Stat", menuName = "Character_Base_Stat/MakeNewCharacterBase", order = 0)]
public class Character_SO : ScriptableObject
{
    [SerializeField]
    GameObject _characterSource;

    GameObject _copySource = null;
    public GameObject CharacterSource { get { return _copySource; } }




    public string[] _skillScriptNames;


    public string _characterName;

    public float _maxHP;

    public float _Atk;


    public float _Def;

    public float _criticalDamage;

    public float _criticalPercentage;

    public float _speed;


    //성장치
    public float _maxHPbyLevel;

    public float _AtkbyLevel;

    public float _DefbyLevel;


    public void SourceRender()
    {
        if (_characterSource != null)
        {
            _copySource = Instantiate(_characterSource);
        }
        
        _copySource.AddComponent<Character_Base>();
        _copySource.name = _characterName;
        SetSkillAtCopySource();
    }

    private void SetSkillAtCopySource()
    {
        if (_copySource == null)
            return;

        for (int i = 0; i < _skillScriptNames.Length; i++)
        {
            _copySource.AddComponent(System.Type.GetType(_skillScriptNames[i]));
        }
    }
}
