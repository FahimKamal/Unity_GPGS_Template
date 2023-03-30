using UnityEngine;

public class MeFirst : MonoBehaviour
{
    [SerializeField] private GameObject retryWidow;

    private void OnEnable()
    {
        PlayServiceManager.Instance.onSignedIn += OnSignedIn;
        PlayServiceManager.Instance.onDataLoaded += DataLoaded;
        PlayServiceManager.Instance.noDataFound += NoDataFound;
        PlayServiceManager.Instance.onDataLoadFailed += OnDataLoadFailed;
        PlayServiceManager.Instance.onSignInFailed += OnSignInFailed;
    }

    private void OnDisable()
    {
        PlayServiceManager.Instance.onSignedIn -= OnSignedIn;
        PlayServiceManager.Instance.onDataLoaded -= DataLoaded;
        PlayServiceManager.Instance.noDataFound -= NoDataFound;
        PlayServiceManager.Instance.onDataLoadFailed -= OnDataLoadFailed;
        PlayServiceManager.Instance.onSignInFailed -= OnSignInFailed;
    }

    private void OnSignInFailed()
    {
        var retryWin = Instantiate(retryWidow);
        retryWin.GetComponent<LoginFailed>().Set("LogIn Failed",
            "Failed to login. Check your internet connection.");
    }

    private void OnDataLoadFailed()
    {
        var retryWin = Instantiate(retryWidow);
        retryWin.GetComponent<LoginFailed>().Set("Data Load Failed",
            "Failed to load data from cloud. Check your internet connection.");
    }


    private void OnSignedIn()
    {
        if (!PlayServiceManager.Instance.editorMode)
            // load data from cloud.
            PlayServiceManager.Instance.OpenSave(false);
        else
        {
            // Editor mode is active in play service manager.
            //GetComponent<SceneTransition>().PerformTransition();
        }
    }

    private void DataLoaded()
    {
        //GetComponent<SceneTransition>().PerformTransition();
    }

    private void NoDataFound()
    {
        // no data found in cloud.

        //GetComponent<SceneTransition>().PerformTransition();
    }
}