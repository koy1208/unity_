using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Knite Skill3
public class Skill_WideArea_Attack : MonoBehaviour, Skill_Interface
{
    SkillManager.SKILL_TARGET _skillTarget;
    public SkillManager.SKILL_TARGET Skill_Target { get { return _skillTarget; } }

    public int SkillCount { get; set; }

    string _skillName;
    public string SkillName { get { return _skillName; } }

    float _Atk;
    float _CriticalDamage;
    bool _isCri = false;

    GameObject _Target;


    void Start()
    {
        _skillTarget = SkillManager.SKILL_TARGET.ENEMY_ALL;
        _skillName = "전체 공격";
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
            this.KniteSkill3GiveDamage();
            StageManager._instance.StageStateTurnEnd();
        }
        else
        {
            animator.SetTrigger("Skill3");
        }

    }


    public void KniteSkill3GiveDamage()
    {
        GameObject[] Party = null;

        var TargetAI = _Target.GetComponent<Character_Base>();

        switch (TargetAI.Owner)
        {
            case Character_Base.CHARACTER_OWNER.PLAYER:
                Party = StageManager._instance.GetPlayerParty();
                break;

            case Character_Base.CHARACTER_OWNER.ENEMY:
                Party = StageManager._instance.GetEnemyParty();
                break;
        }

        if (Party == null)
            return;

        float damage = _Atk * 0.7f;

        for (int i = 0; i < Party.Length; i++)
        {
            if (Party[i] == null)
                continue;

            var CB = Party[i].GetComponent<Character_Base>();

            if (CB.IsDead() == true)
                continue;

            if (_isCri == true)
                CB.Damaged(damage, _CriticalDamage, _isCri);
            else
                CB.Damaged(damage);
        }

    }

    public void KniteSkill3StageStateTurnEnd()
    {
        StageManager._instance.StageStateTurnEnd();
    }
}
