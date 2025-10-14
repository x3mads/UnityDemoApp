using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class ShowErrorDto
    {
        internal const int UNEXPECTED = -1;
        internal const int SHOW_ERROR_NOT_REQUESTED = 0;
        internal const int SHOW_ERROR_NO_LONGER_AVAILABLE = 1;
        internal const int SHOW_ERROR_LOADING = 2;
        internal const int SHOW_ERROR_SHOW_FAILED = 3;
        internal const int SHOW_ERROR_ALREADY_USED = 4;

        internal int Type { get; }
        internal string Message { get; }
        internal int? AdapterCode { get; }
        internal string ErrorName { get; }

        internal ShowErrorDto(
            int type,
            string message,
            int? adapterCode,
            string errorName
        )
        {
            Type = type;
            Message = message;
            AdapterCode = adapterCode;
            ErrorName = errorName;
        }

        internal static ShowErrorDto FromAndroidJavaObject(AndroidJavaObject androidJavaObject)
        {
            using (androidJavaObject)
            {
                return new ShowErrorDto(
                    type: androidJavaObject.Call<int>(methodName: "getType"),
                    message: androidJavaObject.Call<string>(methodName: "getMessage"),
                    adapterCode: AdapterCodeFromAndroidJavaObject(androidJavaObject.Call<AndroidJavaObject>("getAdapterCode")),
                    errorName: androidJavaObject.Call<string>("getErrorName")
                );
            }
        }

        private static int? AdapterCodeFromAndroidJavaObject([CanBeNull]AndroidJavaObject javaObject)
        {
            if (javaObject == null) return null;
            using (javaObject)
            {
                return javaObject.Call<int>("intValue");    
            }
            
        }

        internal ShowError ToShowError()
        {
            switch (Type)
            {
                case SHOW_ERROR_NOT_REQUESTED:
                    return new ShowError(ShowError.ErrorType.NotRequested, Message);
                case SHOW_ERROR_NO_LONGER_AVAILABLE:
                    return new ShowError(ShowError.ErrorType.NoLongerAvailable, Message);
                case SHOW_ERROR_LOADING:
                    return new ShowError(ShowError.ErrorType.Loading, Message);
                case SHOW_ERROR_SHOW_FAILED:
                    return new ShowError(ShowError.ErrorType.ShowFailed, Message, AdapterCode, ErrorName);
                case SHOW_ERROR_ALREADY_USED:
                    return new ShowError(ShowError.ErrorType.AlreadyUsed, Message);
                case UNEXPECTED:
                    return new ShowError(ShowError.ErrorType.Unexpected, Message);
                default:
                    return new ShowError(ShowError.ErrorType.Unexpected, Message);
            }
        }
    }
}