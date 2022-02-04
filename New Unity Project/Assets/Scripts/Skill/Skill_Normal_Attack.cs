using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//대부분의 캐릭터의 스킬1
public class Skill_Normal_Attack : MonoBehaviour, Skill_Interface
{
    int _skillCount;
    public int SkillCount { get { return _skillCount; } set { _skillCount = value; } }

    string _skillName;
    public string SkillName { get { return _skillName; } }

    SkillManager.SKILL_TARGET _skillTarget;
    public SkillManager.SKILL_TARGET Skill_Target { get { return _skillTarget; } }


    float _Atk;
    float _CriticalDamage;
    bool _isCri = false;

    GameObject _Target;

    void Start()
    {
        _skillTarget = SkillManager.SKILL_TARGET.ENEMY_ONE;
        _skillName = "일반 공격";
    }



    public void UseSkill(GameObject CharactersUsingSkill, GameObject TargetObject)
    {
        if (TargetObject == null)
        {
            var user = CharactersUsingSkill.GetComponent<Character_Base>();
            int rnd = 0;
            switch (user.Owner)
            {
                case Character_Base.CHARACTER_OWNER.ENEMY:
                    var playerParty = StageManager._instance.GetPlayerParty();

                    while (TargetObject == null)
                    {
                        rnd = Random.Range(0, playerParty.Length);
                        if (playerParty[rnd] != null)
                            if (playerParty[rnd].GetComponent<Character_Base>().IsDead() == false)
                                TargetObject = playerParty[rnd];
                    }

                    break;

                case Character_Base.CHARACTER_OWNER.PLAYER:
                    var enemyParty = StageManager._instance.GetEnemyParty();

                    while (TargetObject == null)
                    {
                        rnd = Random.Range(0, enemyParty.Length);
                        if (enemyParty[rnd] != null)
                            if (enemyParty[rnd].GetComponent<Character_Base>().IsDead() == false)
                                TargetObject = enemyParty[rnd];
                    }

                    break;
            }
            user.setTarget(TargetObject);
        }

        var OwnerAI = CharactersUsingSkill.GetComponent<Character_Base>();

        _Atk = OwnerAI.ATK;
        _CriticalDamage = OwnerAI.CriticalDamage;
        _isCri = OwnerAI.CriticalPercentage >= Random.Range(1, 101);

        _Target = TargetObject;

        Animator animator = OwnerAI.GetComponent<Animator>();

        if (animator == null)
        {
            this.NormalAttackGiveDamage();
            StageManager._instance.StageStateTurnEnd();
        }
        else
        {
            animator.SetTrigger("Skill1");
        }

    }

    
    public void NormalAttackGiveDamage()
    {
        var TargetAI = _Target.GetComponent<Character_Base>();

        if (_isCri == true)
            TargetAI.Damaged(_Atk, _CriticalDamage, _isCri);
        else
            TargetAI.Damaged(_Atk);
    }

    public void NormalAttackStageStateTurnEnd()
    {
        StageManager._instance.StageStateTurnEnd();

        this.GetComponent<Animator>().ResetTrigger("Skill1");
    }
    
}
