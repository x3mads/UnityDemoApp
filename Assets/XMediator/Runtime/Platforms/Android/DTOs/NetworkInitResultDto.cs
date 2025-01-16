using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class NetworkInitResultDto
    {
        internal bool IsSuccess { get; }
        internal string Name { get; }
        internal InstanceErrorDto Error { get; }

        internal NetworkInitResultDto(bool isSuccess, string name, InstanceErrorDto error)
        {
            IsSuccess = isSuccess;
            Name = name;
            Error = error;
        }

        internal NetworkInitResult ToNetworkInitResult()
        {
            return IsSuccess
                ? (NetworkInitResult)new NetworkInitResult.Success(
                    name: Name,
                    description: $"{Name} success"
                )
                : new NetworkInitResult.Failure(
                    name: Name,
                    description: $"{Name} failed with error {Error.Message}",
                    error: Error.ToInstanceError()
                );
        }

        internal static NetworkInitResultDto FromAndroidJavaObject(AndroidJavaObject androidJavaObject)
        {
            using (androidJavaObject)
            {
                return new NetworkInitResultDto(
                    isSuccess: androidJavaObject.Get<bool>("isSuccess"),
                    name: androidJavaObject.Call<string>("getName"),
                    error: InstanceErrorDto.FromAndroidJavaObject(androidJavaObject.Get<AndroidJavaObject>("error"))
                );
            }
        }
    }
}