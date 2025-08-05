using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    public class PurchaseEventDto
    {
        private static string PURCHASE_EVENT_DTO_CLASSNAME = "com.etermax.android.xmediator.unityproxy.dto.PurchaseEventDto";

        public static AndroidJavaObject From(PurchaseEvent purchaseEvent)
        {
            return new AndroidJavaObject(
                PURCHASE_EVENT_DTO_CLASSNAME,
                purchaseEvent.Amount,
                purchaseEvent.Currency,
                purchaseEvent.Sku,
                purchaseEvent.Name
            );
        }
    }
}