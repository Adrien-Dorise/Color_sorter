using UnityEngine;
using UnityEngine.Advertisements;

//https://docs.unity.com/ads/en-us/manual/UnityDeveloperIntegrations
//https://docs.unity.com/ads/en-us/manual/InitializingTheUnitySDK
//https://www.youtube.com/watch?v=SBtfuWEN5qk
//https://www.youtube.com/watch?v=tzgOTVPXC-I

public class ad_init : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

    [SerializeField] ad_reward rewardScript;
    [SerializeField] ad_video videoScript;
    [SerializeField] ad_banner bannerScript;
 
    void Awake()
    {
        InitializeAds();
    }
 
    public void InitializeAds()
    {
    #if UNITY_IOS
            _gameId = _iOSGameId;
    #elif UNITY_ANDROID
            _gameId = _androidGameId;
    #elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
    #endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

 
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        videoScript.LoadAd();

    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}