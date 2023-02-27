using TMPro;
using UnityEngine;

public class SavedGamesUI : MonoBehaviour
{
    public string playerName;
    public int age;
    
    public TextMeshProUGUI logTxt;
    public TextMeshProUGUI outputTxt;
    public TMP_InputField nameInputField;
    public TMP_InputField ageInputField;

    public void OnValueChangeName(string field)
    {
        playerName = nameInputField.text;
    }
    
    public void OnValueChangeAge(string field)
    {
        age = int.Parse(ageInputField.text);
    }

    public void PrintOutput()
    {
        outputTxt.text = "";
        outputTxt.text += "My name is "+playerName + "\n";
        outputTxt.text += "My age is "+age + "\n";
    }

    public void PrintLog(string log)
    {
        logTxt.text += "\n" + log;
    }

    public void ClearLog()
    {
        logTxt.text = "Log Text";
        outputTxt.text = "Output Text";
        nameInputField.text = "";
        ageInputField.text = "";
    }
}
