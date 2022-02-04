using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnOnOff : MonoBehaviour
{
    public enum OnOff
    {
        ON,
        OFF
    }

    OnOff _curState = OnOff.OFF;
    Color _color;

    private void Start()
    {
        _curState = OnOff.OFF;
        _color = Color.white;
    }

    public void setBtnOnOff(OnOff onoff)
    {
        if(_color == new Color(0,0,0,0))
            _color = this.GetComponent<Image>().color;

        _curState = onoff;
        this.SetColor();
    }

    private void SetColor()
    {
        switch (_curState)
        {
            case OnOff.ON:
                _curState = OnOff.OFF;
                this.GetComponent<Image>().color = _color / 2;
                break;

            case OnOff.OFF:
                _curState = OnOff.ON;
                this.GetComponent<Image>().color = _color;
                break;
        }
    }

    
    public void OnclickButton()
    {
        /*
        
        switch (_curState)
        {
            case OnOff.ON:
                _curState = OnOff.OFF;
                this.GetComponent<Image>().color = _color / 2;
                break;

            case OnOff.OFF:
                _curState = OnOff.ON;
                this.GetComponent<Image>().color = _color;
                break;
        }
        */
    }
}
