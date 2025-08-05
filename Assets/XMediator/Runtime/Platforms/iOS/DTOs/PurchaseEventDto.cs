using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class PurchaseEventDto
    {
        [SerializeField] internal double amount;
        [SerializeField] internal string currency;
        [SerializeField] internal NullableObject<string> sku;
        [SerializeField] internal NullableObject<string> name;

        internal PurchaseEventDto(double amount, string currency, NullableObject<string> sku, NullableObject<string> name)
        {
            this.amount = amount;
            this.currency = currency;
            this.sku = sku;
            this.name = name;
        }

        internal static PurchaseEventDto FromPurchaseEvent(PurchaseEvent purchaseEvent)
        {
            return new PurchaseEventDto(
                purchaseEvent.Amount,
                purchaseEvent.Currency,
                new NullableObject<string>(purchaseEvent.Sku),
                new NullableObject<string>(purchaseEvent.Name)
            );
        }
        
        internal string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}