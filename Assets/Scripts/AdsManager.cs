using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour {
    // Android
    // 배너 광고   ca-app-pub-3940256099942544/6300978111
    // 전면 광고   ca-app-pub-3940256099942544/1033173712
    // 보상형 광고  ca-app-pub-3940256099942544/5224354917

    // iOS
    // 배너 광고   ca-app-pub-3940256099942544/2934735716
    // 전면 광고   ca-app-pub-3940256099942544/4411468910
    // 보상형 광고  ca-app-pub-3940256099942544/1712485313

    private static readonly string AND_BANNER_ID = "ca-app-pub-7832687788012663/9321714808";
    private static readonly string AND_INTERSTITIAL_ID = "ca-app-pub-7832687788012663/1332806899";
    private static readonly string AND_REWARD_ID = "ca-app-pub-7832687788012663/3605180232";

    private static readonly string iOS_BANNER_ID = "ca-app-pub-7832687788012663/6504877560";
    private static readonly string iOS_INTERSTITIAL_ID = "ca-app-pub-7832687788012663/1679720432";
    private static readonly string iOS_REWARD_ID = "ca-app-pub-7832687788012663/8965994142";

    //private static readonly string iOS_BANNER_ID = "ca-app-pub-3940256099942544/2934735716";
    //private static readonly string iOS_INTERSTITIAL_ID = "ca-app-pub-3940256099942544/4411468910";
    //private static readonly string iOS_REWARD_ID = "ca-app-pub-3940256099942544/1712485313";

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
    private InterstitialAd interstitialAd;

    private RewardedAd coinRewardedAd;
    private RewardedAd rankUpItemRewardedAd;
    private RewardedAd destroyItemRewardedAd;

    private bool isPrevSoundOn = false;

    public void Start() {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => {
        });

        RequestBannerAd();
        RequestRewardedAd();
        RquestInterstitialAd();
    }

    private void RequestBannerAd() {
        if (!isPremium) {
#if UNITY_ANDROID
            string adUnitId = AND_BANNER_ID;
#elif UNITY_IPHONE
            string adUnitId = iOS_BANNER_ID;
#else
            string adUnitId = "unexpected_platform";
#endif

            if(bannerView != null) {
                bannerView.Destroy();
            }

            this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

            this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
            this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;

            this.bannerView.LoadAd(CreateAdRequest());
        }
    }

    private AdRequest CreateAdRequest() {
        return new AdRequest.Builder().Build();
    }

    private void RquestInterstitialAd() {
#if UNITY_ANDROID
        string adUnitId = AND_INTERSTITIAL_ID;
#elif UNITY_IPHONE
        string adUnitId = iOS_INTERSTITIAL_ID;
#else
        string adUnitId = "unexpected_platform";
#endif
        if (interstitialAd != null) {
            interstitialAd.Destroy();
        }

        this.interstitialAd = new InterstitialAd(adUnitId);

        this.interstitialAd.OnAdClosed += this.HandleOnAdClosed;
        this.interstitialAd.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;

        this.interstitialAd.LoadAd(CreateAdRequest());
    }

    public void ShowInterstitialAd() {
        if (interstitialAd.IsLoaded() && !isPremium) {
            interstitialAd.Show();
        }
    }

    public void HandleOnAdClosed(object sender, EventArgs args) {
        RquestInterstitialAd();
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        Debug.Log($"{sender} loaded is fail : {args.Message}");
    }

    public void HandleOnAdLoaded(object sender, EventArgs arge) {
        AboveAds();
    }

    private void AboveAds() {
        Vector3 adsAbovePos = new Vector3(0, this.bannerView.GetHeightInPixels() / 200, 0);
        GameManager.init.premiumGround.transform.position += adsAbovePos;
        ObjectManager.init._currBackground.transform.position += adsAbovePos;
    }

    private void RequestRewardedAd() {
        this.coinRewardedAd = CreateAndLoadRewardedAd();
        this.coinRewardedAd.OnUserEarnedReward += HandleUserCoinReward;

        this.rankUpItemRewardedAd = CreateAndLoadRewardedAd();
        this.rankUpItemRewardedAd.OnUserEarnedReward += HandleUserRankUpItemReward;

        this.destroyItemRewardedAd = CreateAndLoadRewardedAd();
        this.destroyItemRewardedAd.OnUserEarnedReward += HandleUserDestroyItemReward;
    }

    public RewardedAd CreateAndLoadRewardedAd() {
#if UNITY_ANDROID
        string adUnitId = AND_REWARD_ID;
#elif UNITY_IPHONE
        string adUnitId = iOS_REWARD_ID;
#else
        string adUnitId = "unexpected_platform";
#endif
        RewardedAd rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoaded;

        rewardedAd.LoadAd(CreateAdRequest());
        return rewardedAd;
    }

    public void ShowAdCoinRewarded() {
        if (this.coinRewardedAd.IsLoaded() && UIManager.init.isEnableCoinAds) {
            this.coinRewardedAd.Show();
        }
    }

    public void ShowAdRankUpItem() {
        if (this.rankUpItemRewardedAd.IsLoaded()) {
            if(ObjectManager.init.TargetOfRankUpItem().Count <= 0)
                UIManager.init.itemUnavailableMessage.SetActive(true);
            else 
                this.rankUpItemRewardedAd.Show();
        }
    }

    public void ShowAdDestroyItem() {
        if (this.destroyItemRewardedAd.IsLoaded()) {
            if (ObjectManager.init.TargetOfDestroyItem().Count <= 0)
                UIManager.init.itemUnavailableMessage.SetActive(true);
            else 
                this.destroyItemRewardedAd.Show();
        }
    }

    public void HandleUserCoinReward(object sender, Reward args) {
        ScoreManager.init.AddCoin(300);
        UIManager.init.SetCoinAdsTimer();
    }

    public void HandleUserRankUpItemReward(object sender, Reward args) {
        ObjectManager.init.RankUpItem(true);
    }

    public void HandleUserDestroyItemReward(object sender, Reward args) {
        ObjectManager.init.DestroyItem(true);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args) {
        SoundOff();
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args) {
        if (sender == coinRewardedAd) {
            coinRewardedAd = CreateAndLoadRewardedAd();
            this.coinRewardedAd.OnUserEarnedReward += HandleUserCoinReward;
        } else if (sender == rankUpItemRewardedAd) {
            rankUpItemRewardedAd = CreateAndLoadRewardedAd();
            this.rankUpItemRewardedAd.OnUserEarnedReward += HandleUserRankUpItemReward;
        } else if (sender == destroyItemRewardedAd) {
            destroyItemRewardedAd = CreateAndLoadRewardedAd();
            this.destroyItemRewardedAd.OnUserEarnedReward += HandleUserDestroyItemReward;
        }

        SoundOn();
    }

    public void HandleRewardedAdFailedToLoaded(object sender, AdErrorEventArgs args) {
        Debug.Log($"{sender} loaded is fail : {args.Message}");
    }

    public void DestroyBannerAd() {
        try {
            this.bannerView.Destroy();
        } catch (Exception e) {
            Debug.Log(e.StackTrace);
        }
        finally {
            isPremium = true;
        }
    }

    private void SoundOn() {
        if (isPrevSoundOn)
            SettingManager.init.BGMOn(false);
        isPrevSoundOn = false;
    }

    private void SoundOff() {
        isPrevSoundOn = !SettingManager.init.isBGMOn;
        SettingManager.init.BGMOn(true);
    }
}

