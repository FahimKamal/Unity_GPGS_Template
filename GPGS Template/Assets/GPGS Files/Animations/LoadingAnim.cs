using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingAnim : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        StartCoroutine(LoadingAnimation());
    }

    private void OnDisable()
    {
        StopCoroutine(LoadingAnimation());
    }

    IEnumerator LoadingAnimation()
    {
        while (true)
        {
            text.text = "Loading";
            yield return new WaitForSeconds(0.1f);
            text.text = "Loading .";
            yield return new WaitForSeconds(0.1f);
            text.text = "Loading ..";
            yield return new WaitForSeconds(0.1f);
            text.text = "Loading ...";
        }
    }
}
