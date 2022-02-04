using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneUIManager : MonoBehaviour
{
    [SerializeField]
    InputField _IDField;
    [SerializeField]
    InputField _PWFeild;

    [SerializeField]
    Text _notifyText;




    public void OnClickLogInBtn()
    {
        if (_IDField.text == "")
        {
            StartCoroutine(ShowNotifyText("ID를 입력 해주세요"));
            return;
        }

        if (_PWFeild.text == "")
        {
             StartCoroutine(ShowNotifyText("PW를 입력 해주세요"));
            return;
        }

        string id = _IDField.text;
        JsonSingleton.SaveJsonFile saveFile = null;


        try
        {
            saveFile = JsonSingleton._instance.LoadJsonFile<JsonSingleton.SaveJsonFile>(Application.dataPath, id);
        }
        catch (System.Exception err)
        {
            StartCoroutine(ShowNotifyText("ID또는 PW를 다시 입력해주십시오."));
            return;
        }


        string pw = _PWFeild.text;

        if(saveFile.CheckPW(pw))
        {
            StartCoroutine(ShowNotifyText("로그인 성공"));
            //로비로 이동

            SceneManager.LoadScene("LobyScene");
            Player._instance.InitPlayer(saveFile);
        }
        else
        {
            StartCoroutine(ShowNotifyText("ID또는 PW를 다시 입력해주십시오."));
            return;
        }
    }

    public void OnClickMakeNewAccountBtn()
    {
        if (_IDField.text == "")
        {
            StartCoroutine(ShowNotifyText("ID를 입력 해주세요"));
            return;
        }

        if (_PWFeild.text == "")
        {
            StartCoroutine(ShowNotifyText("PW를 입력 해주세요"));
            return;
        }

        JsonSingleton.SaveJsonFile saveFile = null;

        try
        {
            saveFile = JsonSingleton._instance.LoadJsonFile<JsonSingleton.SaveJsonFile>(Application.dataPath, _IDField.text);
        }
        catch (System.Exception err)
        {
            var newSave = new JsonSingleton.SaveJsonFile(_IDField.text, _PWFeild.text);
            string data = JsonSingleton._instance.ObjectToJson(newSave);
            JsonSingleton._instance.CreateJsonFile(Application.dataPath, _IDField.text, data);
            Debug.Log(data);
            StartCoroutine(ShowNotifyText("ID가 생성되었습니다."));
            return;
        }

        if(saveFile != null)
        {
            StartCoroutine(ShowNotifyText("이미 존재하는 ID입니다"));
            return;
        }
    }

    IEnumerator ShowNotifyText(string text)
    {
        _notifyText.text = text;
        _notifyText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        _notifyText.gameObject.SetActive(false);
    }
}
