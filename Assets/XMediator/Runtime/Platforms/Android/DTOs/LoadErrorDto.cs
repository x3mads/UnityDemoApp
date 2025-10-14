using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class LoadErrorDto
    {
        internal const int UNEXPECTED = 0;
        internal const int REQUEST_FAILED = 1;
        internal const int NO_FILL = 2;
        internal const int ALREADY_USED = 3;

        internal int Type { get; }
        internal string Message { get; }

        internal LoadErrorDto(
            int type,
            string message
        )
        {
            Type = type;
            Message = message;
        }


        internal static LoadErrorDto FromAndroidJavaObject(AndroidJavaObject loadError)
        {
            using(loadError)
            {
                return new LoadErrorDto(
                    type: loadError.Call<int>("getType"),
                    message: loadError.Call<string>("getMessage")
                );
            }
        }

        internal LoadError ToLoadError()
        {
            switch (Type)
            {
                case REQUEST_FAILED:
                    return new LoadError(LoadError.ErrorType.RequestFailed, Message);
                case NO_FILL:
                    return new LoadError(LoadError.ErrorType.NoFill, Message);
                case ALREADY_USED:
                    return new LoadError(LoadError.ErrorType.AlreadyUsed, Message);
                case UNEXPECTED:
                    return new LoadError(LoadError.ErrorType.Unexpected, Message);
                default:
                    return new LoadError(LoadError.ErrorType.Unexpected, "Unexpected error code");
            }
        }
    }
}