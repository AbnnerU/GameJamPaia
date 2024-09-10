
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Ads
{
    public class AdRequest : MonoBehaviour
    {
        [SerializeField] private AdConfig adConfig;
        [SerializeField] private AdsChannel adsChannel;
        [SerializeField] private bool tryLoadOnAwake;
        [SerializeField] private bool playOnEndLoad;
        [SerializeField] private AdEvent OnLoadAdSuccess;
        [SerializeField] private AdEvent OnShowAdCompleted;
        [SerializeField] private AdEvent OnLoadAdFail;
        [SerializeField] private AdEvent OnShowAdFail;

        private void Start()
        {
            if (tryLoadOnAwake)
                adsChannel.AdLoadRequest(adConfig, ctx => OnLoadCompleted(ctx));
        }

        public void LoadAd()
        {
            adsChannel.AdLoadRequest(adConfig, ctx => OnLoadCompleted(ctx));
        }

        public void OnLoadCompleted(bool loadsuccessfully)
        {
            if (loadsuccessfully)
            {
                OnLoadAdSuccess?.Invoke();

                if (playOnEndLoad == false)
                    return;

                ShowAd();
            }
            else
            {
                OnLoadAdFail?.Invoke();
            }
        }

        public void ShowAd()
        {
            Debug.Log("Show Ad");
            adsChannel.OnAdShowRequest(adConfig, ctx => OnShowCompleted(ctx));
        }

        public void OnShowCompleted(bool showAdsuccessfully)
        {
            Debug.Log("Ad Request ," + gameObject.name + ", show status (" + showAdsuccessfully + ")");

            if (showAdsuccessfully)
                OnShowAdCompleted?.Invoke();
            else
                OnShowAdFail?.Invoke();

        }



        [Serializable]
        class AdEvent : UnityEvent { }
    }
}