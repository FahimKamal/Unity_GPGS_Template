using System;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// This class works same as Unity's default PlayerPrefs class. You can save bool, int,
/// float and string data with a string key. You can also make it run time only. While
/// in run time mode class will temporary save data in ran and after closing the game
/// all data will be lost. Otherwise this class will save data in a json file.
/// </summary>
public class SaveGameManager : MonoBehaviour
{
    #region Singleton

    public static SaveGameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mFileName = Path.Combine(Application.persistentDataPath, "GameData.json");
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    private string mFileName;

    [Tooltip("If active Game will save game data while playing. After restarting the game all data will be lost.")]
    [SerializeField] private bool runtimeOnly;


    public GameDataClass Storage { get; private set; }

    private void Start()
    {
        Storage = runtimeOnly ? new GameDataClass() : Load();
    }

    #region Check methods

    /// <summary>
    /// Return TRUE if the file exists.
    /// </summary>
    /// <returns></returns>
    private bool FileExists()
    {
        return File.Exists(mFileName);
    }

    /// <summary>
    /// Checks if a key exists in the storage or not.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private bool KeyExists(DataType type, string key)
    {
        if (Storage == null) return false;
        switch (type)
        {
            case DataType.Boolean:
                return Storage.boolData.ContainsKey(key);
            case DataType.Integer:
                return Storage.intData.ContainsKey(key);
            case DataType.Float:
                return Storage.floatData.ContainsKey(key);
            case DataType.String:
                return Storage.stringData.ContainsKey(key);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    #endregion
    
    
    /// <summary>
    /// Load data from storage.
    /// </summary>
    /// <returns></returns>
    private GameDataClass Load()
    {
        // If the file doesn't exist, the method just returns false. A warning message is written into the Error property.
        if (!FileExists())
        {
            PopupManager.Instance.ShowPopup("The file " + mFileName + " doesn't exist.", onlyLog:true);
            return new GameDataClass();
        }

        try
        {
            return LoadJsonFile<GameDataClass>(mFileName);
        }
        catch (Exception e)
        {
            PopupManager.Instance.ShowPopup("This system exception has been thrown during loading: " + e.Message, onlyLog:true);
            throw;
        }

    }

    /// <summary>
    /// Save the data into storage.
    /// </summary>
    /// <returns></returns>
    public bool Save(GameDataClass cloudData = null)
    {
        if (runtimeOnly)
        {
            PopupManager.Instance.ShowPopup("Runtime mode active, not saving to disk.");
            
            if (cloudData == null) return true;
            
            Storage = cloudData;
            PopupManager.Instance.ShowPopup("Game data from cloud is stored in RAM.");
            return true;
        }
        try
        {
            if (cloudData != null)
            {
                SaveJsonFile(mFileName, cloudData);
                PopupManager.Instance.ShowPopup("Game data from cloud is stored in disk.");    
                return true;
            }
            SaveJsonFile(mFileName, Storage);
            return true;
        }
        catch (Exception e)
        {
            PopupManager.Instance.ShowPopup("This system exception has been thrown during saving: " + e.Message, onlyLog:true);
            return false;
        }
            
    }

    #region Set/Get data from Storage

    /// <summary>
    /// Adds Boolean data in storage 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetBool(string key, bool value)
    {
        //var storage = Load();
        if (KeyExists(DataType.Boolean, key))
            Storage.boolData[key] = value;
        else
            Storage.boolData.Add(key, value);
        
        Save();
    }
    
    /// <summary>
    /// Adds Integer data in storage 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetInt(string key, int value)
    {
        //var storage = Load();
        if (KeyExists(DataType.Integer, key))
            Storage.intData[key] = value;
        else
            Storage.intData.Add(key, value);
        
        Save();
    }
    
    /// <summary>
    /// Adds Float data in storage 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetFloat(string key, float value)
    {
        //var storage = Load();
        if (KeyExists(DataType.Float, key))
            Storage.floatData[key] = value;
        else
            Storage.floatData.Add(key, value);
        
        Save();
    }
    
    /// <summary>
    /// Adds Integer data in storage 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetString(string key, string value)
    {
        //var storage = Load();
        if (KeyExists(DataType.String, key))
            Storage.stringData[key] = value;
        else
            Storage.stringData.Add(key, value);
        
        Save();
    }

    /// <summary>
    /// Return the boolean data for the given key (or the defined defaultValue if nothing found).
    /// </summary>
    /// <param name="key">The key to search in storage.</param>
    /// <param name="defaultValue">If key doesn't exists, return this default value.</param>
    /// <returns>Value from storage.</returns>
    public bool GetBool(string key, bool defaultValue = false)
    {
        //var storage = Load();
        try
        {
            return KeyExists(DataType.Boolean, key) ? Storage.boolData[key] : defaultValue;
        }
        catch (Exception)
        {
            PopupManager.Instance.ShowPopup("GetBool error using key: " + key);
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Return the integer data for the given key (or the defined defaultValue if nothing found).
    /// </summary>
    /// <param name="key">The key to search in storage.</param>
    /// <param name="defaultValue">If key doesn't exists, return this default value.</param>
    /// <returns>Value from storage.</returns>
    public int GetInt(string key, int defaultValue = 0)
    {
        //var storage = Load();
        try
        {
            return KeyExists(DataType.Integer,key) ? Storage.intData[key] : defaultValue;
        }
        catch (Exception)
        {
            PopupManager.Instance.ShowPopup("GetInt error using key: " + key);
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Return the float data for the given key (or the defined defaultValue if nothing found).
    /// </summary>
    /// <param name="key">The key to search in storage.</param>
    /// <param name="defaultValue">If key doesn't exists, return this default value.</param>
    /// <returns>Value from storage.</returns>
    public float GetFloat(string key, float defaultValue = 0f)
    {
        //var storage = Load();
        try
        {
            return KeyExists(DataType.Float ,key) ? Storage.floatData[key] : defaultValue;
        }
        catch (Exception)
        {
            PopupManager.Instance.ShowPopup("GetFloat error using key: " + key);
            return defaultValue;
        }
    }

    /// <summary>
    /// Return the string data for the given key (or the defined defaultValue if nothing found).
    /// </summary>
    /// <param name="key">The key to search in storage.</param>
    /// <param name="defaultValue">If key doesn't exists, return this default value.</param>
    /// <returns>Value from storage.</returns>
    public string GetString(string key, string defaultValue = "")
    {
        //var storage = Load();
        try
        {
            return KeyExists(DataType.String, key) ? Storage.stringData[key] : defaultValue;
        }
        catch (Exception)
        {
            PopupManager.Instance.ShowPopup("GetString error using key: " + key);
            return defaultValue;
        }
    }

    #endregion

    #region Read/Write data to disk

    /// <summary>
    /// Saves the specified data to a json file at the specified path.
    /// </summary>
    /// <param name="path">The path to the json file.</param>
    /// <param name="data">The data to save.</param>
    /// <typeparam name="T">The type of the data to serialize to the file.</typeparam>
    private static void SaveJsonFile<T>(string path, T data) where T : class
    {
        fsData serializedData;
        var serializer = new fsSerializer();
        serializer.TrySerialize(data, out serializedData).AssertSuccessWithoutWarnings();
        var file = new StreamWriter(path);
        var json = fsJsonPrinter.PrettyJson(serializedData);
        file.WriteLine(json);
        file.Close();
    }
    
    /// <summary>
    /// Loads the json file at the specified path.
    /// </summary>
    /// <param name="path">The path to the json file.</param>
    /// <typeparam name="T">The type of the data to which to deserialize the file to.</typeparam>
    /// <returns></returns>
    private static T LoadJsonFile<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            PopupManager.Instance.ShowPopup("File not found at path: " + path);
            PopupManager.Instance.ShowPopup("Creating new file at path: " + path);
            //InitData();
        }

        var file = new StreamReader(path);
        var fileContents = file.ReadToEnd();
        var data = fsJsonParser.Parse(fileContents);
        object deserialized = null;
        var serializer = new fsSerializer();
        serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();
        file.Close();
        
        return deserialized as T;
    }

    #endregion
   
    
}
