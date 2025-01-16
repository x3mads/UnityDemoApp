using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal class XMediatorUnityInitCallback : AndroidJavaProxy
        {
            private const string UNITY_INIT_CALLBACK_CLASSNAME =
                "com.etermax.android.xmediator.unityproxy.initialize.UnityInitCallback";

            private readonly Action<InitResult> _action;

            internal XMediatorUnityInitCallback(Action<InitResult> action) : base(UNITY_INIT_CALLBACK_CLASSNAME)
            {
                _action = action;
            }

            public AndroidJavaObject onInitCompleted(AndroidJavaObject initResult)
            {
                _action.Invoke(AndroidProxyMapper.ParseOnInitCompleted(initResult));
                return null;
            }
        
    }
}