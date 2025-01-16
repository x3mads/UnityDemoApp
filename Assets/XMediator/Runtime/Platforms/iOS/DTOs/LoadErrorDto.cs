using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class LoadErrorDto
    {
        [SerializeField] internal int errorCode;
        [SerializeField] internal string errorDescription;

        internal LoadError ToLoadError()
        {
            switch (errorCode)
            {
                case (int) LoadError.ErrorType.RequestFailed:
                    return new LoadError(LoadError.ErrorType.RequestFailed, errorDescription);
                case (int) LoadError.ErrorType.NoFill:
                    return new LoadError(LoadError.ErrorType.NoFill, errorDescription);
                case (int) LoadError.ErrorType.AlreadyUsed:
                    return new LoadError(LoadError.ErrorType.AlreadyUsed, errorDescription);
                case (int) LoadError.ErrorType.Unexpected:
                    return new LoadError(LoadError.ErrorType.Unexpected, errorDescription);
                default:
                    return new LoadError(LoadError.ErrorType.Unexpected, "Unexpected error code");
            }
        }
    }

    [Serializable]
    internal class FailedToLoadDto
    {
        [SerializeField] internal LoadErrorDto error;
        [SerializeField] internal LoadResultDto result;

        internal LoadError ToLoadError()
        {
            return error.ToLoadError();
        }

        internal LoadResult ToLoadResult()
        {
            return result.ToLoadResult();
        }
    }
}