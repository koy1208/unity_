using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stage_UI : MonoBehaviour
{
    static public Stage_UI _instance = null;
    [SerializeField]
    Text _StageEndText;

    [SerializeField]
    GameObject _StageEndImage;

    [SerializeField]
    Button _buttonSource;

    [SerializeField]
    ScrollRect _dropItemScrollView;

    Sprite[] _dropItemSprites;

    bool _init = false;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        _dropItemSprites = new Sprite[50];
        _init = false;
    }

    public void SetStageEndUI()
    {
        _StageEndImage.SetActive(true);
        this.SetStageEndText();
        if (_init == false)
        {
            this.SetDropItemScrollView();
            _init = true;
        }
    }

    private void SetStageEndText()
    {
        _StageEndText.text = GameManager._instance.StageName;
    }

    private void SetDropItemScrollView()
    {
        var layout = _dropItemScrollView.GetComponentInChildren<HorizontalLayoutGroup>();

        for (int i = 0; i < _dropItemSprites.Length; i++)
        {
            if (_dropItemSprites[i] == null)
                break;

            var content = Instantiate(_buttonSource);
            content.transform.parent = layout.gameObject.transform;
            content.image.sprite = _dropItemSprites[i];
        }

        var recttransform = _dropItemScrollView.GetComponent<RectTransform>();
        recttransform.rect.Set(0, 0, 80 * _dropItemSprites.Length, recttransform.rect.height);
    }

    public void AddDropItemSprite(Sprite itemSprite)
    {
        if (itemSprite == null)
            return;

        for (int i = 0; i < _dropItemSprites.Length; i++)
        {
            if (_dropItemSprites[i] != null)
                continue;

            _dropItemSprites[i] = itemSprite;
            break;
        }
    }

    public void OnClickRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickGoMainButton()
    {
        // GameManager._instance.SetCurSceneState(GameManager.SceneState.LOBYINIT);
        SceneManager.LoadScene("LobyScene");
    }
}
