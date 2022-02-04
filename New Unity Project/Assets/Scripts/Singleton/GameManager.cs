using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public GameManager _instance = null;

    [SerializeField]
    string[] _stageArray;
    public string[] Stages { get { return _stageArray; } }

    string _stageName;
    public string StageName { get { return _stageName; } }

    private void Awake()
    {
        if (_instance == null)
        {
            //_curScene = SceneState.LOBYINIT;
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }



    //마우스 클릭 확인후 호출
    //if(Input.GetMouseButtonDowm(0))
    //  GetHitByTouchedMouse();
    //**************************
    public RaycastHit GetHitByTouchedMouse()
    {
        RaycastHit hit;
        Ray MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(MouseRay, out hit, 100.0f);

        return hit;
    }

    public void SetStageName(string name)
    {
        _stageName = name;
    }

}
