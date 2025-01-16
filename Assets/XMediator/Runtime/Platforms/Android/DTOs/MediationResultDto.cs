using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XMediator.Api;

namespace XMediator.Android
{
    internal class MediationResultDto
    {
        internal IEnumerable<NetworkInitResultDto> Networks { get; }

        internal MediationResultDto(IEnumerable<NetworkInitResultDto> networks)
        {
            Networks = networks;
        }

        internal static MediationResultDto FromAndroidJavaObject(AndroidJavaObject androidJavaObject)
        {
            using(androidJavaObject)
            {
                return new MediationResultDto(
                    networks: androidJavaObject
                        .Call<AndroidJavaObject[]>("getNetworks")
                        .Select(NetworkInitResultDto.FromAndroidJavaObject).ToList()
                );
            }
        }

        internal MediationResult ToMediationResult()
        {
            return new MediationResult(
                networks: Networks.Select(e => e.ToNetworkInitResult()).ToArray()
            );
        }
    }
}