using Assets.Scripts.Ads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelPlayAds : ManagerBase<LevelPlayAds>
{
    [SerializeField] private AdsChannel adsChannel;
    string appKey = "1f92570bd";
    bool ready = false;

    private Action<bool> AdLoadedAction = null;
    private Action<bool> AdShowAction = null;

    private void Awake()
    {
        Setup(this);
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
       

        //Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

        adsChannel.OnAdloadRequest += Channel_OnAdLoadRequest;
        adsChannel.OnAdShowRequest += Channel_OnAdShowRequest;
    }

    private void OnDestroy()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent -= SdkInitializationCompletedEvent;

        //Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent -= InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent -= InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent -= InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent -= InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent -= InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent -= InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent -= InterstitialOnAdClosedEvent;

        adsChannel.OnAdloadRequest -= Channel_OnAdLoadRequest;
        adsChannel.OnAdShowRequest -= Channel_OnAdShowRequest;
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    void Start()
    {
        IronSource.Agent.init(appKey, IronSourceAdUnits.INTERSTITIAL);
        IronSource.Agent.validateIntegration();
    }

    private void SdkInitializationCompletedEvent()
    {
        Debug.Log("INITIALIZATION COMPLETED");
        print("INITIALIZATION COMPLETED");
        ready = true;
    }

    private void Channel_OnAdLoadRequest(AdConfig config, Action<bool> OnAdLoadCallBack)
    {
        Debug.Log("Load AD request. Is ready? :" + ready);
        if (ready)
        {
            if (AdLoadedAction != null) return;
            IronSource.Agent.loadInterstitial();

            AdLoadedAction = OnAdLoadCallBack;
        }
        else
        {
            IronSource.Agent.init(appKey, IronSourceAdUnits.INTERSTITIAL);

            AdLoadedAction = null;

            OnAdLoadCallBack?.Invoke(false);
        }
    }

    private void Channel_OnAdShowRequest(AdConfig config, Action<bool> OnAdShowCallBack)
    {
        Debug.Log("Show AD request. Is ready? :" + ready);
        if (ready)
        {
            if (AdShowAction != null) return;
            IronSource.Agent.showInterstitial();

            AdShowAction = OnAdShowCallBack;
        }
        else
        {
            IronSource.Agent.init(appKey, IronSourceAdUnits.INTERSTITIAL);

            AdLoadedAction = null;

            OnAdShowCallBack?.Invoke(false);
        }
    }



    /************* Interstitial AdInfo Delegates *************/
    // Invoked when the interstitial ad was loaded succesfully.
    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {
        if (AdLoadedAction == null) return;

        Debug.Log("Load ad succesfully");

        AdLoadedAction?.Invoke(true);

        AdLoadedAction = null;
    }
    // Invoked when the initialization process has failed.
    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        if (AdLoadedAction == null) return;

        Debug.Log("Load ad Fail");
        Debug.Log(ironSourceError);

        AdLoadedAction?.Invoke(false);

        AdLoadedAction = null;
    }
    // Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        //
    }
    // Invoked when end user clicked on the interstitial ad
    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        //
    }
    // Invoked when the ad failed to show.
    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        if (AdShowAction == null) return;

        Debug.Log("Show ad Fail");
        Debug.Log(ironSourceError);

        AdShowAction?.Invoke(false);

        AdShowAction = null;
    }
    // Invoked when the interstitial ad closed and the user went back to the application screen.
    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        if (AdShowAction == null) return;

        Debug.Log("Show ad succesfully");

        AdShowAction?.Invoke(true);

        AdShowAction = null;
    }
    // Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
    // This callback is not supported by all networks, and we recommend using it only if  
    // it's supported by all networks you included in your build. 
    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {
    }



}
