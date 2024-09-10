using System;
using UnityEngine;

namespace Assets.Scripts.Ads
{
    [CreateAssetMenu(fileName = "AdsChannel", menuName = "Assets/AdsChannel")]
    public class AdsChannel : ScriptableObject
    {
        public Action<AdConfig,Action<bool>> OnAdShowRequest;
        public Action<AdConfig,Action<bool>> OnAdloadRequest;

        public void AdLoadRequest(AdConfig adConfig, Action<bool> OnLoadCompleted)
        {
            if(OnAdloadRequest != null)
            {
                OnAdloadRequest?.Invoke(adConfig, OnLoadCompleted);
            }
        }
        public void AdShowRequest(AdConfig adConfig, Action<bool> OnLoadCompleted)
        {
            if (OnAdShowRequest != null)
            {
                OnAdShowRequest?.Invoke(adConfig, OnLoadCompleted);
            }
        }
    }
}

