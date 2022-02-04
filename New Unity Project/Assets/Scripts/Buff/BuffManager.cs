using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffManager : MonoBehaviour
{
    static public BuffManager _instance = null;

    public enum BUFF
    {
        INCREASE_ATK,
        POISON
    }

    private enum BUFF_STATE
    {
        USE,
        DELETE
    }

    [SerializeField]
    Sprite[] _buffSprite;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    public void BuffEffect(BUFF buff, GameObject target)
    {
        switch (buff)
        {
            case BUFF.INCREASE_ATK:
                IncreaseATK(target, BUFF_STATE.USE);
                break;

            case BUFF.POISON:
                Poison(target, BUFF_STATE.USE);
                break;
        }
    }

    public void BuffEffectDelete(BUFF buff, GameObject target)
    {
        switch (buff)
        {
            case BUFF.INCREASE_ATK:
                IncreaseATK(target, BUFF_STATE.DELETE);
                break;

            case BUFF.POISON:
                Poison(target, BUFF_STATE.DELETE);
                break;
        }
    }

    private void IncreaseATK(GameObject target, BUFF_STATE state)
    {
        float atk = target.GetComponent<Character_Base>().ATK;

        if (state == BUFF_STATE.USE)
        {
            target.GetComponent<Character_Base>().ATK *= 1.3f;
        }
        else
        {
            target.GetComponent<Character_Base>().ATK /= 1.3f;
        }
    }

    public Sprite GetBuffSprite(BUFF buff)
    {
        return _buffSprite[(int)buff];
    }

    private void Poison(GameObject target, BUFF_STATE state)
    {
        if (state == BUFF_STATE.USE)
        {
            target.GetComponent<Character_Base>().Damaged(20, 0.0f, false, true, true);
        }
    }
}
