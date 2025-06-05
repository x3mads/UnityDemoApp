using System;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class InAppPurchaseSummaryDto
    {
        [SerializeField] internal NullableDoubleValue totalAmountSpent;
        [SerializeField] internal NullableObject<string> currencyCode;
        [SerializeField] internal NullableIntValue numberOfPurchases;

        private InAppPurchaseSummaryDto(NullableDoubleValue totalAmountSpent,
            NullableObject<string> currencyCode, NullableIntValue numberOfPurchases)
        {
            this.totalAmountSpent = totalAmountSpent;
            this.currencyCode = currencyCode;
            this.numberOfPurchases = numberOfPurchases;
        }

        [CanBeNull]
        internal static InAppPurchaseSummaryDto FromInAppPurchaseSummary(InAppPurchaseSummary inAppPurchaseSummary)
        {
            if (inAppPurchaseSummary == null)
            {
                return null;
            }
            
            return new InAppPurchaseSummaryDto(
                new NullableDoubleValue((double?)inAppPurchaseSummary.TotalAmountSpent),
                new NullableObject<string>(inAppPurchaseSummary.CurrencyCode),
                new NullableIntValue(inAppPurchaseSummary.NumberOfPurchases));
        }
        
        internal InAppPurchaseSummary ToInAppPurchaseSummary()
        {
            return new InAppPurchaseSummary(
                (decimal?)totalAmountSpent.GetValue(),
                currencyCode.GetValue(),
                numberOfPurchases.GetValue());
        }
    }
}