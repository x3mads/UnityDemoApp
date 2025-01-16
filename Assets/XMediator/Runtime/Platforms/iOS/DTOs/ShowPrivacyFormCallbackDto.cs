using System;
using UnityEngine;

namespace XMediator.iOS
{
    [Serializable]
    internal class ShowPrivacyFormCallbackDto
    {
        [SerializeField] internal bool isNull;
        [SerializeField] internal string reason;

        internal Exception ToException()
        {
            return isNull ? null : new Exception(reason);
        }
    }
}