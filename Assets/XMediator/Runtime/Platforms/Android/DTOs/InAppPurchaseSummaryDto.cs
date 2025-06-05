using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class InAppPurchaseSummaryDto
    {
        private const string IN_APP_PURCHASE_SUMMARY_CLASS_NAME = "com.etermax.android.xmediator.unityproxy.dto.InAppPurchaseSummaryDto";
        
        internal decimal? TotalAmountSpent { get; }
        [CanBeNull] internal string CurrencyCode { get; }
        internal int? NumberOfPurchases { get; }

        private InAppPurchaseSummaryDto(decimal? totalAmountSpent, [CanBeNull] string currencyCode, int? numberOfPurchases)
        {
            TotalAmountSpent = totalAmountSpent;
            CurrencyCode = currencyCode;
            NumberOfPurchases = numberOfPurchases;
        }

        internal static InAppPurchaseSummaryDto From(InAppPurchaseSummary inAppPurchaseSummary)
        {
            return new InAppPurchaseSummaryDto(
                totalAmountSpent: inAppPurchaseSummary.TotalAmountSpent,
                currencyCode: inAppPurchaseSummary.CurrencyCode,
                numberOfPurchases: inAppPurchaseSummary.NumberOfPurchases
            );
        }

        internal static InAppPurchaseSummaryDto From([CanBeNull] AndroidJavaObject androidJavaObject)
        {
            if (androidJavaObject == null) return null;
            using (androidJavaObject)
            {
                return new InAppPurchaseSummaryDto(
                    totalAmountSpent: Utils.DecimalFromAndroidJavaObject(androidJavaObject.Call<AndroidJavaObject>("getTotalAmountSpent")),
                    currencyCode: androidJavaObject.Call<string>("getCurrencyCode"),
                    numberOfPurchases: Utils.IntFromAndroidJavaObject(androidJavaObject.Call<AndroidJavaObject>("getNumberOfPurchases"))
                );
            }
        }
        
        public AndroidJavaObject ToAndroidJavaObject()
        {
            return new AndroidJavaObject(
                IN_APP_PURCHASE_SUMMARY_CLASS_NAME,
                Utils.ToAndroidDouble(TotalAmountSpent),
                CurrencyCode,
                Utils.ToAndroidInt(NumberOfPurchases)
            );
        }


        public InAppPurchaseSummary ToInAppPurchaseSummary()
        {
            return new InAppPurchaseSummary(
                totalAmountSpent: TotalAmountSpent,
                currencyCode: CurrencyCode,
                numberOfPurchases: NumberOfPurchases
            );
        }
    }
}
