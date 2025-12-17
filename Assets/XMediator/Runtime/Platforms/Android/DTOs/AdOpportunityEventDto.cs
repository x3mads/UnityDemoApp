using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    public class AdOpportunityEventDto
    {
        private static string AD_OPPORTUNITY_EVENT_DTO_CLASSNAME = "com.etermax.android.xmediator.unityproxy.dto.AdOpportunityEventDto";

        public static AndroidJavaObject From(AdOpportunityEvent adOpportunityEvent)
        {
            return new AndroidJavaObject(
                AD_OPPORTUNITY_EVENT_DTO_CLASSNAME,
                ToAdTypeString(adOpportunityEvent.AdType),
                adOpportunityEvent.AdSpace
            );
        }

        private static string ToAdTypeString(AdType adType)
        {
            switch (adType)
            {
                case AdType.Interstitial: return "itt";
                case AdType.Rewarded: return "rew";
                case AdType.AppOpen: return "apo";
                case AdType.Banner: return "ban";
            }
            return "unhandled";
        }
    }
}