using System;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class InitResultDto
    {
        internal const int RESULT_SUCCESS = 0;
        internal const int RESULT_REQUEST_FAILED = 1;
        internal const int RESULT_RE_INIT = 2;

        internal int Result { get; }
        internal string SessionId { get; }
        internal string Message;

        internal InitResultDto(int result, string sessionId, string message)
        {
            Result = result;
            SessionId = sessionId;
            Message = message;
        }

        internal static InitResultDto FromAndroidJavaObject(AndroidJavaObject androidJavaObject)
        {
            using (androidJavaObject)
            {
                var result = androidJavaObject.Call<int>("getResult");
                return new InitResultDto(
                    result: result,
                    sessionId: androidJavaObject.Call<string>("getSessionId"),
                    message: androidJavaObject.Call<string>("getMessage")
                );
            }
        }

        internal InitResult ToInitResult()
        {
            switch (Result)
            {
                case RESULT_SUCCESS:
                    return new InitResult.Success(
                        sessionId: SessionId
                    );
                case RESULT_REQUEST_FAILED:
                case RESULT_RE_INIT:
                    return new InitResult.Failure(
                        errorCode: Result,
                        errorDescription: Message
                    );
                default:
                    throw new Exception("Invalid init result");
            }
        }
    }
}