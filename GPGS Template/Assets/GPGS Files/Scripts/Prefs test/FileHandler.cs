using System;
using System.IO;
using FullSerializer;
using UnityEngine;

public static class FileHandler
{
    public static readonly string FileName = Path.Combine(Application.persistentDataPath, "GameData.json");

    #region Check methods

    /// <summary>
    /// Return TRUE if the file exists.
    /// </summary>
    /// <returns></returns>
    public static bool FileExists()
    {
        return File.Exists(FileName);
    }

    /// <summary>
    /// Checks if a key exists in the storage or not.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="key"></param>
    /// <param name="storage"></param>
    /// <returns></returns>
    private static bool KeyExists(DataType type, string key, GameDataClass storage)
    {
        if (storage == null) return false;
        switch (type)
        {
            case DataType.Boolean:
                return storage.boolData.ContainsKey(key);
            case DataType.Integer:
                return storage.intData.ContainsKey(key);
            case DataType.Float:
                return storage.floatData.ContainsKey(key);
            case DataType.String:
                return storage.stringData.ContainsKey(key);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    #endregion
    
    
    /// <summary>
    /// Load data from storage.
    /// </summary>
    /// <returns></returns>
    public static GameDataClass Load()
    {
        // If the file doesn't exist, the method just returns false. A warning message is written into the Error property.
        if (!FileExists())
        {
            PopupManager.Instance.ShowPopup("The file " + FileName + " doesn't exist.", onlyLog:true);
            return new GameDataClass();
        }

        try
        {
            return LoadJsonFile<GameDataClass>(FileName);
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
    /// <param name="storage"></param>
    /// <returns></returns>
    public static bool Save(GameDataClass storage)
    {
        try
        {
            SaveJsonFile(FileName, storage);
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
    public static void SetBool(string key, bool value)
    {
        var storage = Load();
        if (KeyExists(DataType.Boolean, key, storage))
            storage.boolData[key] = value;
        else
            storage.boolData.Add(key, value);
        
        Save(storage);
    }
    
    /// <summary>
    /// Adds Integer data in storage 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetInt(string key, int value)
    {
        var storage = Load();
        if (KeyExists(DataType.Integer, key, storage))
            storage.intData[key] = value;
        else
            storage.intData.Add(key, value);
        
        Save(storage);
    }
    
    /// <summary>
    /// Adds Float data in storage 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetFloat(string key, float value)
    {
        var storage = Load();
        if (KeyExists(DataType.Float, key, storage))
            storage.floatData[key] = value;
        else
            storage.floatData.Add(key, value);
        
        Save(storage);
    }
    
    /// <summary>
    /// Adds Integer data in storage 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetString(string key, string value)
    {
        var storage = Load();
        if (KeyExists(DataType.String, key, storage))
            storage.stringData[key] = value;
        else
            storage.stringData.Add(key, value);
        
        Save(storage);
    }

    /// <summary>
    /// Return the boolean data for the given key (or the defined defaultValue if nothing found).
    /// </summary>
    /// <param name="key">The key to search in storage.</param>
    /// <param name="defaultValue">If key doesn't exists, return this default value.</param>
    /// <returns>Value from storage.</returns>
    public static bool GetBool(string key, bool defaultValue = false)
    {
        var storage = Load();
        try
        {
            return KeyExists(DataType.Boolean, key, storage) ? storage.boolData[key] : defaultValue;
        }
        catch (Exception)
        {
            Debug.Log("GetBool error using key: " + key);
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Return the integer data for the given key (or the defined defaultValue if nothing found).
    /// </summary>
    /// <param name="key">The key to search in storage.</param>
    /// <param name="defaultValue">If key doesn't exists, return this default value.</param>
    /// <returns>Value from storage.</returns>
    public static int GetInt(string key, int defaultValue = 0)
    {
        var storage = Load();
        try
        {
            return KeyExists(DataType.Integer,key, storage) ? storage.intData[key] : defaultValue;
        }
        catch (Exception)
        {
            Debug.Log("GetInt error using key: " + key);
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Return the float data for the given key (or the defined defaultValue if nothing found).
    /// </summary>
    /// <param name="key">The key to search in storage.</param>
    /// <param name="defaultValue">If key doesn't exists, return this default value.</param>
    /// <returns>Value from storage.</returns>
    public static float GetFloat(string key, float defaultValue = 0f)
    {
        var storage = Load();
        try
        {
            return KeyExists(DataType.Float ,key, storage) ? storage.floatData[key] : defaultValue;
        }
        catch (Exception)
        {
            Debug.Log("GetFloat error using key: " + key);
            return defaultValue;
        }
    }

    /// <summary>
    /// Return the string data for the given key (or the defined defaultValue if nothing found).
    /// </summary>
    /// <param name="key">The key to search in storage.</param>
    /// <param name="defaultValue">If key doesn't exists, return this default value.</param>
    /// <returns>Value from storage.</returns>
    public static string GetString(string key, string defaultValue = "")
    {
        var storage = Load();
        try
        {
            return KeyExists(DataType.String, key, storage) ? storage.stringData[key] : defaultValue;
        }
        catch (Exception)
        {
            Debug.Log("GetString error using key: " + key);
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
    public static void SaveJsonFile<T>(string path, T data) where T : class
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
    public static T LoadJsonFile<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File not found at path: " + path);
            Debug.Log("Creating new file at path: " + path);
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

