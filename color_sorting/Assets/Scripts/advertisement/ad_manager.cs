using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

//https://docs.unity.com/ads/en-us/manual/UnityDeveloperIntegrations
//https://docs.unity.com/ads/en-us/manual/InitializingTheUnitySDK
//https://www.youtube.com/watch?v=SBtfuWEN5qk
//https://www.youtube.com/watch?v=tzgOTVPXC-I

public class ad_manager : MonoBehaviour
{
    [SerializeField] bool display_ads = true; //Set false to remove ads from game  
    [SerializeField] BannerPosition bannerPos = BannerPosition.BOTTOM;

    private int tokenID;
    powerManager powerScript;



    void Awake()
    {
        if(display_ads)
        {
            Advertisements.Instance.Initialize();
        }
    }

    private void Start()
    {
        tokenID = 0;
        if(SceneManager.GetActiveScene().name == "Level")
        {
            powerScript = GameObject.Find("Power Manager").transform.GetComponent<powerManager>();
        }

        if(display_ads)
        {
            adSelection();
        }
    }
 
    /// <summary>
    /// adSelection is where ads' behavior is set throughout each scene.
    /// </summary>
    private void adSelection()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if(currentScene == "Main Menu")
        {
            HideBanner();
            PlayerPrefs.SetInt(save.bannerClick,0);
            PlayerPrefs.SetInt(save.ad_strike, 0);
        }
        

        if(currentScene == "Level Selection")
        {
            int bannerClick = PlayerPrefs.GetInt(save.bannerClick); 
            if(bannerClick < 100)
            {
                launchBanner();
                PlayerPrefs.SetInt(save.bannerClick, bannerClick+1);
            }
            else
            {
                HideBanner();
            }
        }
        
        if(currentScene == "Level")
        {
            if(!PlayerPrefs.HasKey(save.ad_strike))
            {
                PlayerPrefs.SetInt(save.ad_strike,0);
            }

            HideBanner();


            if(PlayerPrefs.GetInt(save.ad_strike) < 3)
            {
                PlayerPrefs.SetInt(save.ad_strike, PlayerPrefs.GetInt(save.ad_strike) + 1);
            }
            else
            {
                if(PlayerPrefs.GetInt(save.currentLevel) >= 8)
                {
                    Debug.Log("Try ad");
                    if(Random.Range(0,10) >= 6)
                    {
                        launchVideo();
                        PlayerPrefs.SetInt(save.ad_strike, 0);
                    }
                }
            }
        }       
    }

    /// <summary>
    /// Video ad behavior
    /// </summary>
    public void launchVideo()
    {
        if(display_ads)
        {
            Advertisements.Instance.ShowInterstitial();
        }
    }
 
    /// <summary>
    /// Banner ad behavior
    /// </summary>
    public void launchBanner()
    {
        if(display_ads)
        {
            Advertisements.Instance.ShowBanner(bannerPos);
        }
    }

    /// <summary>
    /// Remove displayed banner ad
    /// </summary>
    public void HideBanner()
    {
        Advertisements.Instance.HideBanner();
    }

    /// <summary>
    /// Reward ad beahvior.
    /// Tokens are given to the player when ad is watched entirely
    /// </summary>
    /// <param name="ID">Token ID to increase</param>
    public void launchReward(int ID)
    {
        if(display_ads)
        {
            tokenID = ID;
            Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
        }
        
    }

    /// <summary>
    /// Give token to player when ad is whatched
    /// </summary>
    /// <param name="completed">Token ID to increase</param>
    private void CompleteMethod(bool completed)
    {
        if (completed)
        {
            powerScript.updateOneToken(tokenID, 5);
            powerScript.setInteractablePowerButtons();
        }
    }

}