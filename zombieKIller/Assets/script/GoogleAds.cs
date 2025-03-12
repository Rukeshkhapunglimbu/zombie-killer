using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleAds : MonoBehaviour
{
    public GameManager manager;

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus =>
        {
            LoadAd();
            LoadRewardedAd();
            LoadInterstitialAd();

        });
    }

    #region Banner 

#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-6756555479457016/2049871858";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
    private string _adUnitId = "unused";
#endif

    BannerView _bannerView;

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Top);
    }

    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    #endregion

    #region Interstitial Ad
    private InterstitialAd _interstitialAd;

#if UNITY_ANDROID
    private string _interstialAdUnitId = "ca-app-pub-6756555479457016/5890982318";
#elif UNITY_IPHONE
    private string _interstialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
    private string _interstialAdUnitId = "unused";
#endif

    // private InterstitialAd _interstitialAd;

    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_interstialAdUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
            });
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            Time.timeScale = 0f;
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    // private void RegisterReloadHandler(InterstitialAd interstitialAd)
    // {
    //     // Raised when the ad closed full screen content.
    //     interstitialAd.OnAdFullScreenContentClosed += ()
    //     {
    //         Debug.Log("Interstitial Ad full screen content closed.");

    //         // Reload the ad so that we can show another as soon as possible.
    //         LoadInterstitialAd();
    //     }
    //     ;
    //     // Raised when the ad failed to open full screen content.
    //     interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
    //     {
    //         Debug.LogError("Interstitial ad failed to open full screen content " +
    //                        "with error : " + error);

    //         // Reload the ad so that we can show another as soon as possible.
    //         LoadInterstitialAd();
    //     };
    // }
    #endregion

    #region Rewarded Ad
    // #if UNITY_ANDROID
    //     private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    // #elif UNITY_IPHONE
    //     private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";
    // #else
    //     private string _rewardedAdUnitId = "unused";
    // #endif

    //     private RewardedAd _rewardedAd;


    //     public void LoadRewardedAd()
    //     {
    //         // Clean up the old ad before loading a new one.
    //         if (_rewardedAd != null)
    //         {
    //             _rewardedAd.Destroy();
    //             _rewardedAd = null;
    //         }

    //         Debug.Log("Loading the rewarded ad.");

    //         // Create a request for the ad.
    //         var adRequest = new AdRequest();

    //         // Send the request to load the ad.
    //         RewardedAd.Load(_rewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
    //         {
    //             if (error != null || ad == null)
    //             {
    //                 Debug.LogError("Rewarded ad failed to load: " + error);
    //                 return;
    //             }

    //             Debug.Log("Rewarded ad loaded successfully!");

    //             _rewardedAd = ad;

    //             // ✅ Register event handlers after loading the ad
    //             RegisterEventHandlers(_rewardedAd);
    //         });
    //     }


    //     // public void ShowRewardedAd()
    //     // {
    //     //     const string rewardMsg =
    //     //         "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

    //     //     if (_rewardedAd != null && _rewardedAd.CanShowAd())
    //     //     {
    //     //         _rewardedAd.Show((Reward reward) =>
    //     //         {
    //     //             // TODO: Reward the user.
    //     //             Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
    //     //         });
    //     //     }
    //     // }

    //     public void ShowRewardedAd()
    //     {
    //         if (_rewardedAd != null && _rewardedAd.CanShowAd())
    //         {
    //             _rewardedAd.Show((Reward reward) =>
    //             {
    //                 Debug.Log($"Rewarded Ad Watched! Reward: {reward.Type}, Amount: {reward.Amount}");

    //                 // // Reward the player with 1 extra bullet
    //                 // if (manager != null)
    //                 // {
    //                 //     manager.UpdateBulletUI();
    //                 //     manager.AddExtraBullet();
    //                 // //     manager.HideDefeatPanel();  // Hide the defeat panel after ad is closed
    //                 // //     manager.UpdateBulletUI();  // Update UI to reflect the change
    //                 // }

    //                 // Reload a new ad after watching
    //                 LoadRewardedAd();


    //             });

    //         }
    //         else
    //         {
    //             Debug.Log("Rewarded Ad Not Ready! Reloading...");
    //             LoadRewardedAd();  // Try to reload the ad if it's not available
    //             manager.ShowDefeatPanel(); // Show defeat panel if no ad is available
    //         }
    //     }

    //     private void RegisterEventHandlers(RewardedAd ad)
    //     {
    //         // Raised when the ad is estimated to have earned money.
    //         ad.OnAdPaid += (AdValue adValue) =>
    //         {
    //             Debug.Log(string.Format("Rewarded ad paid {0} {1}.",
    //                 adValue.Value,
    //                 adValue.CurrencyCode));
    //         };
    //         // Raised when an impression is recorded for an ad.
    //         ad.OnAdImpressionRecorded += () =>
    //         {
    //             Debug.Log("Rewarded ad recorded an impression.");
    //         };
    //         // Raised when a click is recorded for an ad.
    //         ad.OnAdClicked += () =>
    //         {
    //             Debug.Log("Rewarded ad was clicked.");
    //         };
    //         // Raised when an ad opened full screen content.
    //         ad.OnAdFullScreenContentOpened += () =>
    //         {
    //             Debug.Log("Rewarded ad full screen content opened.");
    //         };
    //         // Raised when the ad closed full screen content.
    //         ad.OnAdFullScreenContentClosed += () =>
    //         {
    //             Debug.Log("Rewarded Ad Closed.");


    //             if (manager != null)
    //             {
    //                 manager.AddExtraBullet();
    //                 manager.UpdateBulletUI();
    //                 manager.HideDefeatPanel();
    //                 manager.ResumeGame();
    //                 manager.isGameOver = false;
    //                 Time.timeScale = 1f;
    //                 manager.watchAdButton.SetActive(false);
    //             }


    //             // Load a new ad after it's closed
    //             manager.StartCoroutine(DelayedDefeatPanelHide());
    //         };

    //         // Raised when the ad failed to open full screen content.

    //         ad.OnAdFullScreenContentFailed += (AdError error) =>
    //        {
    //            Debug.LogError("Rewarded Ad failed to open: " + error);
    //            LoadRewardedAd(); // Reload the ad if it fails
    //        };
    //     }
    //     #endregion

    //     // Helper Coroutine for iOS UI fix
    //     private IEnumerator DelayedDefeatPanelHide()
    //     {
    //         yield return new WaitForSeconds(0.2f); // Small delay to ensure UI updates correctly
    //         manager.HideDefeatPanel();
    //     }
    // }


  /// <summary>
    /// Loads the rewarded ad
    /// </summary>
    /// 

      // Set Ad Unit ID based on platform
 #if UNITY_ANDROID
        private string _rewardedAdUnitId = "ca-app-pub-6756555479457016/8423708512";
    #elif UNITY_IPHONE
        private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";
    #else
        private string _rewardedAdUnitId = "unused";
    #endif

        private RewardedAd _rewardedAd;
    
    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad...");

        var adRequest = new AdRequest();
        RewardedAd.Load(_rewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                return;
            }

            Debug.Log("Rewarded ad loaded successfully!");
            _rewardedAd = ad;
            RegisterEventHandlers(_rewardedAd);
        });
    }

    /// <summary>
    /// Shows the rewarded ad
    /// </summary>
    public void ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded Ad Watched! Reward: {reward.Type}, Amount: {reward.Amount}");

                LoadRewardedAd();
            });
        }
        else
        {
            Debug.Log("Rewarded Ad Not Ready! Reloading...");
            LoadRewardedAd();
            // manager.StartCoroutine(ShowDefeatWithDelay()); // Delay before showing defeat panel
        }
    }

    /// <summary>
    /// Registers event handlers for the rewarded ad
    /// </summary>
    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad Closed.");

            if (manager != null)
            {
                manager.AddExtraBullet();
                manager.UpdateBulletUI();
                // 🔹 🟢 iOS UI FIX: Small delay before hiding defeat panel
                manager.StartCoroutine(DelayedDefeatPanelHide());
                manager.watchAdButton.SetActive(false);
            }
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded Ad failed to open: " + error);
            LoadRewardedAd();
        };
    }

    /// <summary>
    /// 🟢 Ensures UI updates properly after watching the ad (iOS Fix)
    /// </summary>
    private IEnumerator DelayedDefeatPanelHide()
    {
        yield return new WaitForSeconds(0.5f); // iOS UI sync delay
        if (manager.GetBulletCount() > 0)
        {
            manager.HideDefeatPanel();
            manager.ResumeGame();
            manager.isGameOver = false;
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// 🟢 Prevents instant defeat panel if ad is not ready (iOS Fix)
    /// </summary>
    private IEnumerator ShowDefeatWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Small delay before showing defeat panel
        manager.ShowDefeatPanel();
    }
}
#endregion