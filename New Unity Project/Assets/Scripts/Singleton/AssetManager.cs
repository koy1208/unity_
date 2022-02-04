using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetManager : MonoBehaviour
{
    public static AssetManager _instance = null;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

        }
    }

    public Character_SO GetCharacter_SO(string assetName)
    {
        Character_SO asset;
        string assetPath = "Assets/Resources/ScriptableObject/Character_ScriptableObject/" 
           + assetName + "_SO.asset";
        asset = (Character_SO)ScriptableObject.CreateInstance(typeof(Character_SO));
        asset = AssetDatabase.LoadAssetAtPath<Character_SO>(assetPath);
        asset.SourceRender();

        return asset;
    }
}
