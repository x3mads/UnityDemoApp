using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class PresentErrorDto
    {
        [SerializeField] internal int errorCode;
        [SerializeField] internal string errorDescription;
        [SerializeField] internal int adapterCode;
        
        internal ShowError ToShowError()
        {
            switch (errorCode)
            {
                case (int) ShowError.ErrorType.Unexpected:
                    return new ShowError(ShowError.ErrorType.Unexpected, errorDescription);
                case (int) ShowError.ErrorType.NotRequested:
                    return new ShowError(ShowError.ErrorType.NotRequested, errorDescription);
                case (int) ShowError.ErrorType.NoLongerAvailable:
                    return new ShowError(ShowError.ErrorType.NoLongerAvailable, errorDescription);
                case (int) ShowError.ErrorType.Loading:
                    return new ShowError(ShowError.ErrorType.Loading, errorDescription);
                case (int) ShowError.ErrorType.ShowFailed:
                    return new ShowError(ShowError.ErrorType.ShowFailed, errorDescription, adapterCode);
                case (int) ShowError.ErrorType.AlreadyUsed:
                    return new ShowError(ShowError.ErrorType.AlreadyUsed, errorDescription);
                default:
                    return new ShowError(ShowError.ErrorType.Unexpected, "Unexpected error code");
            }
        }
    }
}