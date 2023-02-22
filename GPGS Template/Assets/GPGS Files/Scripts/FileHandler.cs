using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class FileHandler
{
    private static readonly string FileName = Path.Combine(Application.persistentDataPath, "GameData.dat");
    
    /// <summary>
    /// Return TRUE if the file exists.
    /// </summary>
    /// <returns></returns>
    private static bool FileExists()
    {
        return File.Exists(FileName);
    }
    
    /// <summary>
    /// Checks if a key exists in the storage or not.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="storage"></param>
    /// <returns></returns>
    private static bool KeyExists(string key, Dictionary<string, object> storage)
    {
        return storage != null && storage.ContainsKey(key);
    }

    /// <summary>
    /// Load data from storage.
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, object> Load()
    {
        Dictionary<string, object> storage = new Dictionary<string, object>();
        // If the file doesn't exist, the method just returns false. A warning message is written into the Error property.
        if (!FileExists())
        {
            Debug.Log("The file " + FileName + " doesn't exist.");
            return null;
        }

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream loadFile = File.Open(FileName, FileMode.Open);
            storage = (Dictionary<string, object>)bf.Deserialize(loadFile);
            loadFile.Close();
            
            return storage;
        }
        catch (System.Exception e)
        {
            Debug.Log("This system exception has been thrown during loading: " + e.Message);
            return null;
        }

    }
    
    /// <summary>
    /// Save the data into storage.
    /// </summary>
    /// <param name="storage"></param>
    /// <returns></returns>
    private static bool Save(Dictionary<string, object> storage)
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream saveFile = File.Create(FileName);
            bf.Serialize(saveFile, storage);
            saveFile.Close();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log("This system exception has been thrown during saving: " + e.Message);
            return false;
        }
            
    }
    
    
    
    /// <summary>
    /// Adds data in storage 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void Add(string key, object value)
    {
        var storage = Load() ?? new Dictionary<string, object>();
        if (KeyExists(key, storage))
            storage[key] = value;
        else
            storage.Add(key, value);
        
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
            if (KeyExists(key, storage)) return (bool)storage[key]; else return defaultValue;
        }
        catch (System.Exception)
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
            if (KeyExists(key, storage)) return (int)storage[key]; else return defaultValue;
        }
        catch (System.Exception)
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
            if (KeyExists(key, storage)) return (float)storage[key]; else return defaultValue;
        }
        catch (System.Exception)
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
            if (KeyExists(key, storage)) return (string)storage[key]; else return defaultValue;
        }
        catch (System.Exception)
        {
            Debug.Log("GetString error using key: " + key);
            return defaultValue;
        }
    }
    
}
