using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class InstanceErrorDto {
        [SerializeField] internal int errorCode;
        [SerializeField] internal string errorDescription;
        [SerializeField] internal int adapterCode;
        
        private enum ErrorType
        {
            LoadFailed = 1,
            Timeout = 2,
            InitializationFailed = 3,
            InvalidConfiguration = 4,
            InvalidClassname = 5,
            Unexpected = 6,
            UnsupportedType = 7,
            Skipped = 8,
            ReinitializationUnsupported = 9,
            InitTimeout = 10
        }
        
        internal InstanceError ToInstanceError()
        {
            switch (errorCode)
            {
                case (int)ErrorType.LoadFailed:
                    return new InstanceError(InstanceError.ErrorType.LoadFailed, adapterCode, message: errorDescription);
                case (int)ErrorType.Timeout:
                    return new InstanceError(InstanceError.ErrorType.Timeout, message: errorDescription);
                case (int)ErrorType.InitializationFailed:
                    return new InstanceError(InstanceError.ErrorType.InitializationFailed, message: errorDescription);
                case (int)ErrorType.InvalidClassname:
                    return new InstanceError(InstanceError.ErrorType.InvalidClassname, message: errorDescription);
                case (int)ErrorType.InvalidConfiguration:
                    return new InstanceError(InstanceError.ErrorType.InvalidConfiguration, message: errorDescription);
                case (int)ErrorType.ReinitializationUnsupported:
                    return new InstanceError(InstanceError.ErrorType.ReinitializationUnsupported, message: errorDescription);
                case (int)ErrorType.UnsupportedType:
                    return new InstanceError(InstanceError.ErrorType.UnsupportedType, message: errorDescription);
                case (int)ErrorType.Skipped:
                    return new InstanceError(InstanceError.ErrorType.Skipped, message: errorDescription);
                case (int)ErrorType.Unexpected:
                    return new InstanceError(InstanceError.ErrorType.Unexpected, message: errorDescription);
                case (int)ErrorType.InitTimeout:
                    return new InstanceError(InstanceError.ErrorType.InitTimeout, message: errorDescription);
                default:
                    return new InstanceError(InstanceError.ErrorType.Unexpected, message: "Unexpected error code");
            }
        }
    }
}