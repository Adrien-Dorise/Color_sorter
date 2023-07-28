using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

//https://docs.unity.com/ads/en-us/manual/UnityDeveloperIntegrations
//https://docs.unity.com/ads/en-us/manual/InitializingTheUnitySDK
//https://www.youtube.com/watch?v=SBtfuWEN5qk
//https://www.youtube.com/watch?v=tzgOTVPXC-I

public class ad_manager : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

   // [SerializeField] ad_reward rewardScript;
    ad_video videoScript;
    ad_banner bannerScript;
 
    void Awake()
    {
        InitializeAds();
    }

    private void Start()
    {
        //this.rewardScript = this.GetComponent<ad_reward>();
        this.videoScript = this.GetComponent<ad_video>();
        this.bannerScript = this.GetComponent<ad_banner>();
        adSelection();
    }
 
    private void adSelection()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if(Advertisement.isInitialized)
        {
            if(currentScene == "Level Selection")
            {
                bannerScript.LoadAd();
                bannerScript.ShowBannerAd();
            }
            if(currentScene == "Level")
            {
                videoScript.LoadAd();
                videoScript.ShowAd();
            }
        }
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
        //videoScript.LoadAd();
        //bannerScript.LoadAd();

    }

    public void launchReward()
    {
        //rewardScript.ShowAd();
    }

    public void launchVideo()
    {
        videoScript.ShowAd();
    }
 
    public void launchBanner()
    {
        bannerScript.ShowBannerAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}