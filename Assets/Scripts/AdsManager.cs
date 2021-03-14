using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour {
    // and banner test ca-app-pub-3940256099942544/6300978111
    // and rewardedinterstitial test ca-app-pub-3940256099942544/1033173712
    // and rewarded test ca-app-pub-3940256099942544/5224354917
    private static readonly string ANDROID_BANNER_ID = "ca-app-pub-7832687788012663/9321714808";
    private static readonly string ANDROID_REWARDEDINTERSTITIAL_ID = "ca-app-pub-7832687788012663/8243618613";
    private static readonly string ANDROID_REWARD_ID = "ca-app-pub-7832687788012663/3605180232";

    // and rewarded test ca-app-pub-3940256099942544/4411468910
    // and banner test ca-app-pub-3940256099942544/2934735716
    private static readonly string iOS_REWARDED_ID = "ca-app-pub-7832687788012663/9661809356";
    private static readonly string iOS_BANNER_ID = "ca-app-pub-7832687788012663/6504877560";

    public static AdsManager init = null;
    public bool isPremium = false;

    List<string> deviceIds = new List<string>();

    private void Awake() {
        if (init == null) {
            init = this;
        } else if (init != this) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // ?????? ???? ????

    private BannerView bannerView;
    private RewardedInterstitialAd rewardedInterstitialAd;

    private RewardedAd coinRewardedAd;
    private RewardedAd rankUpItemRewardedAd;
    private RewardedAd destroyItemRewardedAd;

    public void Start() {
        InitAd();
    }

    private void InitAd() {

        //deviceIds.Add("0e3d19f6ba4c353df7c54310bbd28a2f");

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => {
        });

        //Banner
        this.RequestBanner();

        //Rewared
        this.coinRewardedAd = CreateAndLoadRewardedAd();
        this.rankUpItemRewardedAd = CreateAndLoadRewardedAd();
        this.destroyItemRewardedAd = CreateAndLoadRewardedAd();

        //RewardedInterstitial
#if UNITY_ANDROID
        string RIAdUnitId = ANDROID_REWARDEDINTERSTITIAL_ID;
#elif UNITY_IPHONE
        string adUnitId = iOS_REWARDED_ID;
#else
        string adUnitId = "unexpected_platform";
#endif

        AdRequest request = new AdRequest.Builder().Build();
        RewardedInterstitialAd.LoadAd(RIAdUnitId, request, adLoadCallback);
        //this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
    }

    private void adLoadCallback(RewardedInterstitialAd ad, string error) {
        if(error == null) {
            rewardedInterstitialAd = ad;
        }
    }

    public void ShowRewardedInterstitialAd() {
        if(rewardedInterstitialAd != null && !isPremium) {
            rewardedInterstitialAd.Show(userEarnedRewardCallback);
        }
    }

    private void userEarnedRewardCallback(Reward reward) {
        ScoreManager.init.currAdsCount = 0;
    }

    public RewardedAd CreateAndLoadRewardedAd() {
#if UNITY_ANDROID
        string RAdUnitId = ANDROID_REWARD_ID;
#elif UNITY_IPHONE
        string adUnitId = iOS_REWARDED_ID;
#else
        string adUnitId = "unexpected_platform";
#endif
        RewardedAd rewardedAd = new RewardedAd(ANDROID_REWARD_ID);

        //rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        //rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;

        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);

        return rewardedAd;
    }
    public void HandleRewardedAdClosed(object sender, EventArgs args) {
        var ads = (RewardedAd)sender;
        ads = CreateAndLoadRewardedAd();

        if (sender == destroyItemRewardedAd)
            coinRewardedAd = CreateAndLoadRewardedAd();
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args) {
        Debug.Log(sender.GetType());
	}

    public void UserChoseToWatchAd() {
        if (this.coinRewardedAd.IsLoaded()) {
            this.coinRewardedAd.Show();
        }
    }


    private void RequestBanner() {
        Debug.Log("isPremiim - " + isPremium);
        if (!isPremium) {
#if UNITY_ANDROID
            string adUnitId = ANDROID_BANNER_ID;
#elif UNITY_IPHONE
            string adUnitId = iOS_BANNER_ID;
#else
            string adUnitId = "unexpected_platform";
#endif

            // Create a 320x50 banner at the top of the screen.
            this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

            // Callback
            this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
            this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
            this.bannerView.OnAdOpening += this.HandleOnAdOpened;
            this.bannerView.OnAdClosed += this.HandleOnAdClosed;
            this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();

            // Load the banner with the request.
            this.bannerView.LoadAd(request);
        }
    }

    public void HandleOnAdLoaded(object sender, EventArgs args) {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args) {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args) {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args) {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    public void closeAds() {
        try {
            this.bannerView.Destroy();
        } catch (Exception e) {
            Debug.Log(e.StackTrace);
        }
        finally {
            isPremium = true;
        }
    }
}

