
using UnityEngine;

namespace Assets.Scripts.Ads
{
    [CreateAssetMenu(fileName = "AdConfig", menuName = "Assets/AdConfig")]
    public class AdConfig : ScriptableObject
    {
        public AdType adType;
       
    }
    public enum AdType
    {
        Interstitial,
        Rewarded
    }
}

