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
 
    private void adSelection()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if(currentScene == "Main Menu")
        {
            HideBanner();
            PlayerPrefs.SetInt(save.bannerClick,0);
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
            HideBanner();
            if(Random.Range(0,10) >= 7)
            {
                launchVideo();
            }
        }       
    }

    public void launchVideo()
    {
        if(display_ads)
        {
            Advertisements.Instance.ShowInterstitial();
        }
    }
 
    public void launchBanner()
    {
        if(display_ads)
        {
            Advertisements.Instance.ShowBanner(bannerPos);
        }
    }


    public void HideBanner()
    {
        Advertisements.Instance.HideBanner();
    }


    public void launchReward(int ID)
    {
        if(display_ads)
        {
            tokenID = ID;
            Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
        }
        
    }

    private void CompleteMethod(bool completed)
    {
        if (completed)
        {
            powerScript.updateOneToken(tokenID, 5);
            powerScript.setInteractablePowerButtons();
        }
    }

}