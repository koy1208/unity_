using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface Skill_Interface 
{
    SkillManager.SKILL_TARGET Skill_Target { get; }
    int SkillCount { get; set; }
    
    string SkillName { get; }
    void UseSkill(GameObject CharactersUsingSkill, GameObject TargetObject);
}