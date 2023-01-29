using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrefsManager : MonoBehaviour
{
    public static readonly string TestIntValue = "TestIntValue";
    public static readonly string TestFloatValue = "TestFloatValue";
    public static readonly string TestStringValue = "TestStringValue";
    
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TMP_InputField intInput;
    [SerializeField] private TMP_InputField floatInput;
    [SerializeField] private TMP_InputField txtInput;

    [SerializeField] private Button savePrefsBtn;
    [SerializeField] private Button loadPrefsBtn;
    [SerializeField] private Button cloudSaveBtn;
    [SerializeField] private Button cloudLoadBtn;

    private void Awake()
    {
        savePrefsBtn.onClick.AddListener(SavePrefs);
        loadPrefsBtn.onClick.AddListener(LoadPrefs);
        cloudSaveBtn.onClick.AddListener(CloudSave);
        cloudLoadBtn.onClick.AddListener(CloudLoad);
    }

    private void SavePrefs()
    {
        PlayerPrefs.SetInt(TestIntValue, int.Parse(intInput.text));
        PlayerPrefs.SetFloat(TestFloatValue, float.Parse(floatInput.text));
        PlayerPrefs.SetString(TestStringValue, txtInput.text);
        
        description.text = "Value Saved: \n" +"Int: " + intInput.text + "\n" +
            "Float: " + floatInput.text + "\n" +
            "String: " + txtInput.text + "\n";
    }

    private void LoadPrefs()
    {
        var localInt = PlayerPrefs.GetInt(TestIntValue, 0);
        var localFloat = PlayerPrefs.GetInt(TestFloatValue, 0);
        var localString = PlayerPrefs.GetString(TestStringValue, "No Value");
        
        description.text = "Value Loaded: \n" +"Int: " + localInt + "\n" +
                           "Float: " + localFloat + "\n" +
                           "String: " + localString + "\n";
    }

    private void CloudSave()
    {
        
    }

    private void CloudLoad()
    {
        
    }
}
