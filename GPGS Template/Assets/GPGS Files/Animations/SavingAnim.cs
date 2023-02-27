using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SavingAnim : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        StartCoroutine(SavingAnimation());
    }

    private void OnDisable()
    {
        StopCoroutine(SavingAnimation());
    }

    IEnumerator SavingAnimation()
    {
        while (true)
        {
            text.text = "Saving";
            yield return new WaitForSeconds(0.2f);
            text.text = "Saving .";
            yield return new WaitForSeconds(0.2f);
            text.text = "Saving ..";
            yield return new WaitForSeconds(0.2f);
            text.text = "Saving ...";
        }
    }
}
