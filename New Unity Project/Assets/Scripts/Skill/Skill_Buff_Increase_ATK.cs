using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Buff_Increase_ATK : MonoBehaviour, Skill_Interface
{
    int _skillCount;
    public int SkillCount { get { return _skillCount; } set { _skillCount = value; } }

    string _skillName;
    public string SkillName { get { return _skillName; } }


    SkillManager.SKILL_TARGET _skillTarget;
   public SkillManager.SKILL_TARGET Skill_Target { get { return _skillTarget; } }

    void Start()
    {
        _skillName = "공격력 증가";
        _skillTarget = SkillManager.SKILL_TARGET.TEAM_ALL;
    }
    

    public void UseSkill(GameObject CharactersUsingSkill, GameObject TargetObject)
    {
        if(TargetObject == null)
        {
            var user = CharactersUsingSkill.GetComponent<Character_Base>();
            int rnd = 0;
            switch (user.Owner)
            {
                case Character_Base.CHARACTER_OWNER.PLAYER:
                    var playerParty = StageManager._instance.GetPlayerParty();

                    while (TargetObject == null)
                    {
                        rnd = Random.Range(0, playerParty.Length);
                        if(playerParty[rnd] != null)
                           if (playerParty[rnd].GetComponent<Character_Base>().IsDead() == false)
                                TargetObject = playerParty[rnd];
                    }
               
                    break;

                case Character_Base.CHARACTER_OWNER.ENEMY:
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


        var CB = TargetObject.GetComponent<Character_Base>();

        if(CB.Owner == Character_Base.CHARACTER_OWNER.PLAYER)
        {
            GameObject[] party = StageManager._instance.GetPlayerParty();

            for(int i = 0; i < party.Length; i++)
            {
                if (party[i] == null)
                    continue;

                party[i].GetComponent<Character_Base>().AddBuff(BuffManager.BUFF.INCREASE_ATK, 3, false);
            }
        }


        if (CB.Owner == Character_Base.CHARACTER_OWNER.ENEMY)
        {
            GameObject[] party = StageManager._instance.GetEnemyParty();

            for (int i = 0; i < party.Length; i++)
            {
                if (party[i] == null)
                    continue;

                party[i].GetComponent<Character_Base>().AddBuff(BuffManager.BUFF.INCREASE_ATK, 3, false);
            }
        }


        var OwnerAI = CharactersUsingSkill.GetComponent<Character_Base>();

        Animator animator = OwnerAI.GetComponent<Animator>();

        if (animator == null)
        {
            StageManager._instance.StageStateTurnEnd();
        }
        else
        {
            animator.SetTrigger("Skill2");
        }
    }
   
    public void KniteSKill2StageStateTurnEnd()
    {
        StageManager._instance.StageStateTurnEnd();

        this.GetComponent<Animator>().ResetTrigger("Skill2");
    }
}
