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
        PlayServiceManager.Instance.onSignedOut += OnSignedOut;
    }
    
    private void OnDisable()
    {
        PlayServiceManager.Instance.dataSaved -= OnDataSaved;
        PlayServiceManager.Instance.dataLoaded -= OnDataLoaded;
        PlayServiceManager.Instance.onSignedIn -= OnSignedIn;
        PlayServiceManager.Instance.onSignedOut -= OnSignedOut;
    }

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

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject savingTxt;
    [SerializeField] private GameObject loadingTxt;
    [SerializeField] private GameObject signInBtn;
    [SerializeField] private GameObject signOutBtn;

    private void Awake()
    {
        savePrefsBtn.onClick.AddListener(SavePrefs);
        loadPrefsBtn.onClick.AddListener(LoadPrefs);
        cloudSaveBtn.onClick.AddListener(CloudSave);
        cloudLoadBtn.onClick.AddListener(CloudLoad);

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

    private void LoadPrefs()
    {
        var localInt = PlayerPrefs.GetInt(TestIntValue, 0);
        var localFloat = PlayerPrefs.GetFloat(TestFloatValue, 0);
        var localString = PlayerPrefs.GetString(TestStringValue, "No Value");
        
        description.text = "Value Loaded: \n" +"Int: " + localInt + "\n" +
                           "Float: " + localFloat + "\n" +
                           "String: " + localString + "\n";
    }

    private bool isSaving;
    private bool isLoading;
    private void CloudSave()
    {
        StartSaving();
        
        var data = new PrefsData
        {
            intData = PlayerPrefs.GetInt(TestIntValue, 0),
            floatData = PlayerPrefs.GetFloat(TestFloatValue, 0),
            stringData = PlayerPrefs.GetString(TestStringValue)
        };

        var json = JsonUtility.ToJson(data);
        
        PlayServiceManager.Instance.OpenSave(true, json);
    }

    private void CloudLoad()
    {
        StartLoading();
        PlayServiceManager.Instance.OpenSave(false);
    }
    
    private void OnDataSaved()
    {
        StopSaving();
    }

    private void OnDataLoaded()
    {
        description.text = "From Cloud:" + "\n";
        StopLoading();
        var data = PlayServiceManager.Instance.loadedData;
        var formattedData = JsonUtility.FromJson<PrefsData>(data);

        PlayerPrefs.SetInt(TestIntValue, formattedData.intData);
        PlayerPrefs.SetFloat(TestFloatValue, formattedData.floatData);
        PlayerPrefs.SetString(TestStringValue, formattedData.stringData);
        
        LoadPrefs();
    }

    private void OnSignedIn()
    {
        signInBtn.SetActive(false);
        signOutBtn.SetActive(true);
    }
    
    private void OnSignedOut()
    {
        signInBtn.SetActive(true);
        signOutBtn.SetActive(false);
    }

    private void StartSaving()
    {
        isSaving = !isSaving;
        loadingPanel.SetActive(isSaving);
        savingTxt.SetActive(isSaving);
    }

    private void StopSaving()
    {
        isSaving = !isSaving;
        loadingPanel.SetActive(isSaving);
        savingTxt.SetActive(isSaving);
    }

    private void StartLoading()
    {
        isLoading = !isLoading;
        loadingPanel.SetActive(isLoading);
        loadingTxt.SetActive(isLoading);
    }
    

    private void StopLoading()
    {
        isLoading = !isLoading;
        loadingPanel.SetActive(isLoading);
        loadingTxt.SetActive(isLoading);
    }
}

public class PrefsData
{
    public int intData;
    public float floatData;
    public string stringData;
}
