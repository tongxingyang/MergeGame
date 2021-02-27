using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour {
    private static readonly string REWARDED_ID = "ca-app-pub-7832687788012663/3605180232";
    private static readonly string BANNER_ID = "ca-app-pub-7832687788012663/9321714808";
    //private static readonly string REWARDED_ID = "ca-app-pub-3940256099942544/5224354917";
    //private static readonly string BANNER_ID = "ca-app-pub-3940256099942544/6300978111";

    public static AdsManager init = null;
    public bool isPremium = false;

    private void Awake() {
        if (init == null) {
            init = this;
        } else if (init != this) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // 보상형 전면 광고

    private BannerView bannerView;
    private RewardedAd rewardedAd;

    public void Start() {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => {
        
        });
        this.RequestBanner();
        this.rewardedAd = new RewardedAd(REWARDED_ID);

        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;
    }
    public void HandleRewardedAdClosed(object sender, EventArgs args) {
        this.CreateAndLoadRewardedAd();
    }
    public void CreateAndLoadRewardedAd() {
        this.rewardedAd = new RewardedAd(REWARDED_ID);
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
    }

    public void UserChoseToWatchAd() {
        if (this.rewardedAd.IsLoaded() && !isPremium) {
            this.rewardedAd.Show();
        }
    }

    private void RequestBanner() {
        string adUnitId = BANNER_ID;

        if (!isPremium) {
            // Create a 320x50 banner at the top of the screen.
            this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();

            // Load the banner with the request.
            this.bannerView.LoadAd(request);
        }
    }

    public void closeAds() {
        this.bannerView.Destroy();
        isPremium = true;
    }
}
