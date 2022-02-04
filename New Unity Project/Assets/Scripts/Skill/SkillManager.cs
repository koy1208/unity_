using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillManager : MonoBehaviour
{
    static public SkillManager _instance;

    public bool _hitEvent = false;

    public enum SKILL_TARGET
    {
        NONE,
        TEAM_ONE,
        TEAM_ALL,
        ENEMY_ONE,
        ENEMY_ALL,
        ALL
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void HitEvent()
    {
        _hitEvent = true;
    }
}
