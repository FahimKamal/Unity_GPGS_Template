using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrefsManager : MonoBehaviour
{
    private void OnEnable()
    {
        PlayServiceManager.Instance.dataSaved += OnDataSaved;
        PlayServiceManager.Instance.dataLoaded += OnDataLoaded;
        PlayServiceManager.Instance.onSignedIn += OnSignedIn;
        PlayServiceManager.Instance.onSignInFailed += OnSignInFailed;
        PlayServiceManager.Instance.onSignedOut += OnSignedOut;
        PlayServiceManager.Instance.dataSaveFailed += OnDataSaveFailed;
        PlayServiceManager.Instance.dataLoadFailed += OnDataLoadFailed;
    }

    private void OnSignInFailed()
    {
        Instantiate(loginFailedPopup, transform);
    }

    private void OnDisable()
    {
        PlayServiceManager.Instance.dataSaved -= OnDataSaved;
        PlayServiceManager.Instance.dataLoaded -= OnDataLoaded;
        PlayServiceManager.Instance.onSignedIn -= OnSignedIn;
        PlayServiceManager.Instance.onSignInFailed -= OnSignInFailed;
        PlayServiceManager.Instance.onSignedOut -= OnSignedOut;
        PlayServiceManager.Instance.dataSaveFailed -= OnDataSaveFailed;
        PlayServiceManager.Instance.dataLoadFailed -= OnDataLoadFailed;
    }

    private static readonly string TestIntValue = "TestIntValue";
    private static readonly string TestFloatValue = "TestFloatValue";
    private static readonly string TestStringValue = "TestStringValue";
    
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TMP_InputField intInput;
    [SerializeField] private TMP_InputField floatInput;
    [SerializeField] private TMP_InputField txtInput;

    [SerializeField] private Button savePrefsBtn;
    [SerializeField] private Button loadPrefsBtn;
    [SerializeField] private Button cloudSaveBtn;
    [SerializeField] private Button cloudLoadBtn;

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject savingTxt;
    [SerializeField] private GameObject loadingTxt;
    [SerializeField] private GameObject signInBtn;
    [SerializeField] private GameObject signOutBtn;

    [SerializeField] private GameObject loginFailedPopup;

    private void Awake()
    {
        savePrefsBtn.onClick.AddListener(SavePrefs);
        loadPrefsBtn.onClick.AddListener(LoadPrefs);
        cloudSaveBtn.onClick.AddListener(CloudSave);
        cloudLoadBtn.onClick.AddListener(CloudLoad);
        signInBtn.GetComponent<Button>().onClick.AddListener(OnSignInBtnClicked);
        signOutBtn.GetComponent<Button>().onClick.AddListener(OnSignOutBtnClicked);

        if (Social.localUser.authenticated)
        {
            signInBtn.SetActive(false);
            signOutBtn.SetActive(true);
        }
        else
        {
            signInBtn.SetActive(true);
            signOutBtn.SetActive(false);
        }
    }

    private void OnSignOutBtnClicked()
    {
        PlayServiceManager.Instance.SignOutBtn();
    }

    private void OnSignInBtnClicked()
    {
        PlayServiceManager.Instance.BasicSignInBtn();
    }

    /// <summary>
    /// Save data locally with PlayerPrefs
    /// </summary>
    private void SavePrefs()
    {
        PrefsData data = new PrefsData();
        data.intData = int.Parse(intInput.text);
        data.floatData = float.Parse(floatInput.text);
        data.stringData = txtInput.text;
        
        PlayerPrefs.SetInt(TestIntValue, data.intData);
        PlayerPrefs.SetFloat(TestFloatValue, data.floatData);
        PlayerPrefs.SetString(TestStringValue, data.stringData);
        
        description.text = "Value Saved: \n" +"Int: " + intInput.text + "\n" +
            "Float: " + floatInput.text + "\n" +
            "String: " + txtInput.text + "\n";
    }

    /// <summary>
    /// Load data from local PlayerPrefs
    /// </summary>
    private void LoadPrefs()
    {
        var localInt = PlayerPrefs.GetInt(TestIntValue, 0);
        var localFloat = PlayerPrefs.GetFloat(TestFloatValue, 0);
        var localString = PlayerPrefs.GetString(TestStringValue, "No Value");
        
        description.text = "Value Loaded: \n" +"Int: " + localInt + "\n" +
                           "Float: " + localFloat + "\n" +
                           "String: " + localString + "\n";
    }

    private bool mIsSaving;
    private bool mIsLoading;
    
    /// <summary>
    /// Read data from local PlayerPrefs and save them on cloud.
    /// </summary>
    private void CloudSave()
    {
        // Start Saving animation.
        StartSaving();
        
        // Create data class to create json data.
        var data = new PrefsData
        {
            intData = PlayerPrefs.GetInt(TestIntValue, 0),
            floatData = PlayerPrefs.GetFloat(TestFloatValue, 0),
            stringData = PlayerPrefs.GetString(TestStringValue)
        };

        // Create json data.
        var json = JsonUtility.ToJson(data);
        
        // Save the data onto cloud.
        PlayServiceManager.Instance.OpenSave(true, json);
    }
    
    /// <summary>
    /// Will be executed on successful data save onto cloud.
    /// </summary>
    private void OnDataSaved()
    {
        // Stop saving animation.
        StopSaving();
        ClearInputFields();
    }
    
    /// <summary>
    /// Will be executed on failing to save data onto cloud.
    /// </summary>
    private void OnDataSaveFailed()
    {
        StopSaving();
    }

    /// <summary>
    /// Load data from cloud.
    /// </summary>
    private void CloudLoad()
    {
        // Start loading animation.
        StartLoading();
        
        // Load data from cloud
        PlayServiceManager.Instance.OpenSave(false);
    }

    /// <summary>
    /// will be executed on successful data load from cloud.
    /// </summary>
    private void OnDataLoaded()
    {
        description.text = "From Cloud:" + "\n";
        // Stop loading animation.
        StopLoading();
        
        // Get loaded json data from cloud
        var data = PlayServiceManager.Instance.loadedData;
        // Convert the json data indo usable data
        try
        {
            var formattedData = JsonUtility.FromJson<PrefsData>(data);
            // Now save those data back to playerPrefs locally on the device.
            PlayerPrefs.SetInt(TestIntValue, formattedData.intData);
            PlayerPrefs.SetFloat(TestFloatValue, formattedData.floatData);
            PlayerPrefs.SetString(TestStringValue, formattedData.stringData);
        
            // Show the data on screen
            LoadPrefs();
            description.text += "From cloud";
        }
        catch (Exception e)
        {
            PopupManager.Instance.ShowPopup("No data found in This account.", "New Account");
            throw;
        }

    }
    
    /// <summary>
    /// Will be executed on failing to load data from cloud.
    /// </summary>
    private void OnDataLoadFailed()
    {
        PopupManager.Instance.ShowPopup("Data can't be loaded.", "load Failed");
    }

    private void OnSignedIn()
    {
        signInBtn.SetActive(false);
        signOutBtn.SetActive(true);
        CloudLoad();
    }
    
    private void OnSignedOut()
    {
        signInBtn.SetActive(true);
        signOutBtn.SetActive(false);
        ClearInputFields();
    }

    private void ClearInputFields()
    {
        intInput.text = "";
        floatInput.text = "";
        txtInput.text = "";
    }

    #region Saving/Loading animation section

    /// <summary>
    /// Start saving animation.
    /// </summary>
    private void StartSaving()
    {
        mIsSaving = !mIsSaving;
        loadingPanel.SetActive(mIsSaving);
        savingTxt.SetActive(mIsSaving);
    }

    /// <summary>
    /// Stop saving animation.
    /// </summary>
    private void StopSaving()
    {
        mIsSaving = !mIsSaving;
        loadingPanel.SetActive(mIsSaving);
        savingTxt.SetActive(mIsSaving);
    }

    /// <summary>
    /// Start loading animation.
    /// </summary>
    private void StartLoading()
    {
        mIsLoading = !mIsLoading;
        loadingPanel.SetActive(mIsLoading);
        loadingTxt.SetActive(mIsLoading);
    }
    

    /// <summary>
    /// Stop loading animation.
    /// </summary>
    private void StopLoading()
    {
        mIsLoading = !mIsLoading;
        loadingPanel.SetActive(mIsLoading);
        loadingTxt.SetActive(mIsLoading);
    }

    #endregion
}


/// <summary>
/// Data class for storing data onto cloud.
/// </summary>
public class PrefsData
{
    public int intData;
    public float floatData;
    public string stringData;
}
