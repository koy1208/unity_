using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Archer 스킬3
public class Skill_Poison : MonoBehaviour, Skill_Interface
{
    int _skillCount;
    public int SkillCount { get { return _skillCount; } set { _skillCount = value; } }

    string _skillName;
    public string SkillName { get { return _skillName; } }

    SkillManager.SKILL_TARGET _skillTarget;
    public SkillManager.SKILL_TARGET Skill_Target { get { return _skillTarget; } }

    // Start is called before the first frame update
    void Start()
    {
        _skillTarget = SkillManager.SKILL_TARGET.ENEMY_ONE;
        _skillName = "중독 공격";
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

        TargetObject.GetComponent<Character_Base>().AddBuff(BuffManager.BUFF.POISON, 4, true);
    }
}
