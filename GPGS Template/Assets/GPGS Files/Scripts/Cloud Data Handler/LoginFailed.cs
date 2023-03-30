using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginFailed : MonoBehaviour
{
    [SerializeField] private Button retry;
    [SerializeField] private Button quit;
    [SerializeField] private Animation animationCom;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    private void Start()
    {
        retry.onClick.AddListener(RetryBtnPress);
        quit.onClick.AddListener(QuitBtnPress);

        animationCom.Play("Popup Animation on");
    }

    public void Set(string title, string des)
    {
        this.title.text = title;
        description.text = des;
    }

    private void RetryBtnPress()
    {
        StartCoroutine(DestroyPopup());
    }

    private void QuitBtnPress()
    {
        StartCoroutine(DestroyPopup());
        Application.Quit();
    }

    // Destroy the popup after 2 seconds
    private IEnumerator DestroyPopup()
    {
        animationCom.Play("Popup Animation off");
        yield return new WaitForSeconds(0.5f);

#if UNITY_ANDROID
        RestartAndroid();
#else
            Application.Quit();
#endif
    }

    private static void RestartAndroid()
    {
        if (Application.isEditor) return;

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            const int kIntent_FLAG_ACTIVITY_CLEAR_TASK = 0x00008000;
            const int kIntent_FLAG_ACTIVITY_NEW_TASK = 0x10000000;

            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            var intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);

            intent.Call<AndroidJavaObject>("setFlags",
                kIntent_FLAG_ACTIVITY_NEW_TASK | kIntent_FLAG_ACTIVITY_CLEAR_TASK);
            currentActivity.Call("startActivity", intent);
            currentActivity.Call("finish");
            var process = new AndroidJavaClass("android.os.Process");
            var pid = process.CallStatic<int>("myPid");
            process.CallStatic("killProcess", pid);
        }
    }
}