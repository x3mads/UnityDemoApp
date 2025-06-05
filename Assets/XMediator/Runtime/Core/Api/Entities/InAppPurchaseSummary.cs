using System;
using JetBrains.Annotations;

namespace XMediator.Api
{
    public class InAppPurchaseSummary
    {
        /// <summary>
        /// Creates a new <see cref="InAppPurchaseSummary"/> instance.
        /// </summary>
        /// <param name="totalAmountSpent">The total amount the user has spent on in-app purchases. Value is optional and may be null if unknown.</param>
        /// <param name="currencyCode">The currency code (ISO 4217) for the total spent amount. Value is optional and may be null if unknown.</param>
        /// <param name="numberOfPurchases">The total number of in-app purchases the user has made. Value is optional and may be null if unknown.</param>
        public InAppPurchaseSummary(
            [CanBeNull] decimal? totalAmountSpent = null,
            [CanBeNull] string currencyCode = null,
            [CanBeNull] int? numberOfPurchases = null)
        {
            TotalAmountSpent = totalAmountSpent;
            CurrencyCode = currencyCode;
            NumberOfPurchases = numberOfPurchases;
        }

        /// <summary>
        /// The total amount the user has spent on in-app purchases. Value is optional and may be null if unknown.
        /// </summary>
        [CanBeNull] public decimal? TotalAmountSpent { get; }

        /// <summary>
        /// The currency code (ISO 4217) for the total spent amount. Value is optional and may be null if unknown.
        /// </summary>
        [CanBeNull] public string CurrencyCode { get; }
        
        /// <summary>
        /// The total number of in-app purchases the user has made. Value is optional and may be null if unknown.
        /// </summary>
        [CanBeNull] public int? NumberOfPurchases { get; }

        private bool Equals(InAppPurchaseSummary other)
        {
            return TotalAmountSpent == other.TotalAmountSpent && CurrencyCode == other.CurrencyCode && NumberOfPurchases == other.NumberOfPurchases;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((InAppPurchaseSummary) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TotalAmountSpent, CurrencyCode, NumberOfPurchases);
        }
        
        public override string ToString()
        {
            return $"TotalAmountSpent: {TotalAmountSpent}, CurrencyCode: {CurrencyCode}, NumberOfPurchases: {NumberOfPurchases}";
        }
    }

}
