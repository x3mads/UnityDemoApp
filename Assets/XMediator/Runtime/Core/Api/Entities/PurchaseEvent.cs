using System;
using JetBrains.Annotations;

namespace XMediator.Api
{
    /// <summary>
    /// Represents a purchase event for tracking and segmentation.
    /// </summary>
    public class PurchaseEvent
    {
        /// <summary>   
        /// The amount of the purchase.
        /// </summary>
        public double Amount { get; private set; }

        /// <summary>
        /// The currency of the purchase (ISO 4217 code, e.g., "USD").
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Optional. The Stock Keeping Unit identifier. Used for tracking and segmentation.
        /// </summary>
        [CanBeNull] public string Sku { get; private set; }

        /// <summary>
        /// Optional. The display name of the product. Used for tracking only.
        /// </summary>
        [CanBeNull] public string Name { get; private set; }

        /// <summary>
        /// Creates a new <see cref="PurchaseEvent"/> instance.
        /// </summary>
        /// <param name="amount">The amount of the purchase.</param>
        /// <param name="currency">The currency of the purchase (ISO 4217 code, e.g., "USD").</param>
        /// <param name="sku">Optional. The Stock Keeping Unit identifier. Used for tracking and segmentation.</param>
        /// <param name="name">Optional. The display name of the product. Used for tracking only.</param>
        public PurchaseEvent(
            double amount,
            string currency,
            [CanBeNull] string sku = null,
            [CanBeNull] string name = null
        )
        {
            Amount = amount;
            Currency = currency;
            Sku = sku;
            Name = name;
        }

        public override string ToString()
        {
            return $"{nameof(Amount)}: {Amount}, {nameof(Currency)}: {Currency}, {nameof(Sku)}: {Sku}, {nameof(Name)}: {Name}";
        }
    }

}
