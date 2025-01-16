using System;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class InstanceResultDto
    {
        internal const int RESULT_FAILURE = -1;
        internal const int RESULT_SUCCESS = 0;
        internal const int RESULT_UNUSED = 1;

        internal int Result { get; }
        internal string Id { get; }
        internal string Name { get; }
        internal string Classname { get; }
        internal float InformationEcpm { get; }
        internal float? SuccessEcpm { get; }
        internal long? Latency { get; }
        internal string CreativeId { get; }
        internal string SubNetworkName { get; }
        internal string AdNetwork { get; }
        internal string Mediation { get; }
        internal InstanceErrorDto Error { get; }

        internal InstanceResultDto(
            int result,
            string id,
            string name,
            string classname,
            float informationEcpm,
            float? successEcpm,
            long? latency,
            string creativeId,
            string subNetworkName,
            string adNetwork,
            string mediation,
            InstanceErrorDto error)
        {
            Result = result;
            Id = id;
            Name = name;
            Classname = classname;
            InformationEcpm = informationEcpm;
            SuccessEcpm = successEcpm;
            Latency = latency;
            CreativeId = creativeId;
            SubNetworkName = subNetworkName;
            Error = error;
            AdNetwork = adNetwork;
            Mediation = mediation;
        }

        private TimeSpan LatencyAsTimeSpan()
        {
            return Latency != null ? TimeSpan.FromMilliseconds((double) Latency) : TimeSpan.Zero;
        }

        internal static InstanceResultDto FromAndroidJavaObject(AndroidJavaObject androidJavaObject)
        {
            using (androidJavaObject)
            {
                var error = androidJavaObject.Call<AndroidJavaObject>("getError");
                return new InstanceResultDto(
                    result: androidJavaObject.Call<int>(methodName: "getResult"),
                    id: androidJavaObject.Call<string>(methodName: "getId"),
                    name: androidJavaObject.Call<string>("getName"),
                    classname: androidJavaObject.Call<string>("getClassname"),
                    informationEcpm: androidJavaObject.Call<float>("getInformationEcpm"),
                    successEcpm: EcpmFromAndroidJavaObject(androidJavaObject.Call<AndroidJavaObject>("getSuccessEcpm")),
                    latency: LatencyFromAndroidJavaObject(androidJavaObject.Call<AndroidJavaObject>("getLatency")),
                    creativeId: androidJavaObject.Call<string>("getCreativeId"),
                    subNetworkName: androidJavaObject.Call<string>("getSubNetworkName"),
                    error: error != null ? InstanceErrorDto.FromAndroidJavaObject(error) : null,
                    adNetwork: androidJavaObject.Call<string>("getAdNetwork"),
                    mediation: androidJavaObject.Call<string>("getMediation")
                );
            }
        }

        private static long LatencyFromAndroidJavaObject([CanBeNull] AndroidJavaObject javaObject)
        {
            if (javaObject == null) return default;
            using (javaObject)
            {
                return javaObject.Call<long>("longValue");
            }
        }

        private static float EcpmFromAndroidJavaObject([CanBeNull] AndroidJavaObject javaObject)
        {
            if (javaObject == null) return default;
            using (javaObject)
            {
                return javaObject.Call<float>("floatValue");
            }
        }

        internal InstanceResult ToInstanceResult()
        {
            switch (Result)
            {
                case RESULT_FAILURE:
                    return MapInstanceResultFailure(this);
                case RESULT_SUCCESS:
                    return MapInstanceResultSuccess(this);
                case RESULT_UNUSED:
                    return MapInstanceResultUnused(this);
                default:
                    throw new Exception("Invalid result type.");
            }
        }

        private InstanceResult.Failure MapInstanceResultFailure(InstanceResultDto instanceResultDto)
        {
            return new InstanceResult.Failure(
                information: MapInstanceInformation(instanceResultDto),
                latency: instanceResultDto.LatencyAsTimeSpan(),
                error: instanceResultDto.Error.ToInstanceError()
            );
        }

        private static InstanceResult.Success MapInstanceResultSuccess(InstanceResultDto instanceResultDto)
        {
            return new InstanceResult.Success(
                information: MapInstanceInformation(instanceResultDto),
                latency: instanceResultDto.LatencyAsTimeSpan(),
                creativeId: instanceResultDto.CreativeId,
                subNetworkName: instanceResultDto.SubNetworkName,
                ecpm: (decimal) (instanceResultDto.SuccessEcpm ?? 0),
                adNetwork: instanceResultDto.AdNetwork,
                mediation: instanceResultDto.Mediation
            );
        }

        private static InstanceResult.Unused MapInstanceResultUnused(InstanceResultDto instanceResultDto)
        {
            return new InstanceResult.Unused(
                information: MapInstanceInformation(instanceResultDto)
            );
        }

        private static InstanceInformation MapInstanceInformation(InstanceResultDto instanceResultDto) =>
            new InstanceInformation(
                id: instanceResultDto.Id,
                name: instanceResultDto.Name,
                classname: instanceResultDto.Classname,
                ecpm: (decimal) instanceResultDto.InformationEcpm
            );
    }
}