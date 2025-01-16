using System;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class InstanceErrorDto
    {
        internal const int ERROR_LOAD_FAILED = 0;
        internal const int ERROR_INIT_TIMEOUT = 1;
        internal const int ERROR_TIMEOUT = 2;
        internal const int ERROR_INITIALIZATION_FAILED = 3;
        internal const int ERROR_INVALID_CLASSNAME = 4;
        internal const int ERROR_INVALID_CONFIGURATION = 5;
        internal const int ERROR_REINITIALIZATION_UNSUPPORTED = 6;
        internal const int ERROR_UNSUPPORTED_TYPE = 7;
        internal const int ERROR_SKIPPED = 8;
        internal const int ERROR_UNEXPECTED = 9;

        internal int ErrorType { get; }

        internal string Message { get; }

        internal int? AdapterErrorCode { get; }

        internal string AdapterErrorName { get; }

        internal string AdapterErrorReason { get; }

        internal string InvalidConfigurationKey { get; }

        internal string SkippedReason { get; }


        internal InstanceErrorDto(
            int errorType,
            string message,
            int? adapterErrorCode,
            string adapterErrorName,
            string adapterErrorReason,
            string invalidConfigurationKey,
            string skippedReason
        )
        {
            ErrorType = errorType;
            Message = message;
            AdapterErrorCode = adapterErrorCode;
            AdapterErrorName = adapterErrorName;
            AdapterErrorReason = adapterErrorReason;
            InvalidConfigurationKey = invalidConfigurationKey;
            SkippedReason = skippedReason;
        }

        internal static InstanceErrorDto FromAndroidJavaObject([CanBeNull] AndroidJavaObject androidJavaObject)
        {
            if (androidJavaObject == null) return null;
            using (androidJavaObject)
            {
                return new InstanceErrorDto(
                    errorType: androidJavaObject.Call<int>("getErrorType"),
                    message: androidJavaObject.Call<string>("getMessage"),
                    adapterErrorCode: ErrorCodeFromAndroidJavaObject(androidJavaObject.Call<AndroidJavaObject>("getAdapterErrorCode")),
                    adapterErrorName: androidJavaObject.Call<string>("getAdapterErrorName"),
                    adapterErrorReason: androidJavaObject.Call<string>("getAdapterErrorReason"),
                    invalidConfigurationKey: androidJavaObject.Call<string>("getInvalidConfigurationKey"),
                    skippedReason: androidJavaObject.Call<string>("getSkippedReason")
                );
            }
        }

        private static int? ErrorCodeFromAndroidJavaObject([CanBeNull] AndroidJavaObject androidJavaObject)
        {
            if (androidJavaObject == null) return null;
            using (androidJavaObject)
            {
                return androidJavaObject.Call<int>("intValue");    
            }
        }

        internal InstanceError ToInstanceError() =>
            new InstanceError(
                type: MapInstanceErrorErrorType(ErrorType),
                message: Message,
                adapterCode: AdapterErrorCode,
                name: AdapterErrorName
            );

        private InstanceError.ErrorType MapInstanceErrorErrorType(int errorType)
        {
            switch (errorType)
            {
                case ERROR_LOAD_FAILED:
                    return InstanceError.ErrorType.LoadFailed;
                case ERROR_INIT_TIMEOUT:
                    return InstanceError.ErrorType.InitTimeout;
                case ERROR_TIMEOUT:
                    return InstanceError.ErrorType.Timeout;
                case ERROR_INITIALIZATION_FAILED:
                    return InstanceError.ErrorType.InitializationFailed;
                case ERROR_INVALID_CLASSNAME:
                    return InstanceError.ErrorType.InvalidClassname;
                case ERROR_INVALID_CONFIGURATION:
                    return InstanceError.ErrorType.InvalidConfiguration;
                case ERROR_REINITIALIZATION_UNSUPPORTED:
                    return InstanceError.ErrorType.ReinitializationUnsupported;
                case ERROR_UNSUPPORTED_TYPE:
                    return InstanceError.ErrorType.UnsupportedType;
                case ERROR_SKIPPED:
                    return InstanceError.ErrorType.Skipped;
                case ERROR_UNEXPECTED:
                    return InstanceError.ErrorType.Unexpected;
                default:
                    throw new Exception("Invalid error type.");
            }
        }
    }
}