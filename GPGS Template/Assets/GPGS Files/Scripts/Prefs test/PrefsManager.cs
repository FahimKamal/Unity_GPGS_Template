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

    private void CloudSave()
    {
        var data = new PrefsData
        {
            intData = PlayerPrefs.GetInt(TestIntValue, 0),
            floatData = PlayerPrefs.GetFloat(TestFloatValue, 0),
            stringData = PlayerPrefs.GetString(TestStringValue)
        };

        var json = JsonUtility.ToJson(data);
        
        // convert datatype to byte array
        byte[] myData = System.Text.Encoding.ASCII.GetBytes(json);
        Debug.Log(myData);
        
        var loadedData = System.Text.Encoding.ASCII.GetString(myData);

        var dataBack = JsonUtility.FromJson<PrefsData>(loadedData);
        
        Debug.Log(dataBack.intData + "\n" +
                  dataBack.floatData + "\n" +
                  dataBack.stringData + "\n");
    }

    private void CloudLoad()
    {
        
    }
}

public class PrefsData
{
    public int intData;
    public float floatData;
    public string stringData;
}
