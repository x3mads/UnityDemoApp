using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static XMediator.Editor.Tools.MetaMediation.Entities.MetaMediationDependencies;

namespace XMediator.Editor.Android
{
    internal sealed class GoogleAdsNextGenGradleExclusions
    {
        internal const string StartMarker = "// XMediator: Google Mobile Ads Next-Gen exclusions start";
        internal const string EndMarker = "// XMediator: Google Mobile Ads Next-Gen exclusions end";
        private const string NextGenGroup = "com.google.android.libraries.ads.mobile.sdk";
        private const string NextGenArtifact = "ads-mobile-sdk";
        private readonly string _gradleFilePath;

        internal GoogleAdsNextGenGradleExclusions(string gradleFilePath) => _gradleFilePath = gradleFilePath;

        internal static bool RequiresExclusions(IEnumerable<AndroidPackage> packages) =>
            packages != null && packages.Any(package => package != null &&
                package.Group == NextGenGroup && package.Artifact == NextGenArtifact);

        internal bool Apply(bool enabled)
        {
            var originalContent = File.ReadAllText(_gradleFilePath);
            var newline = originalContent.Contains("\r\n") ? "\r\n" : "\n";

            if (!TryRemoveManagedBlocks(originalContent, newline, out var strippedContent))
            {
                return false;
            }

            var desiredContent = enabled
                ? (string.IsNullOrEmpty(strippedContent) ? string.Empty : strippedContent + newline) +
                    ManagedBlock(newline) + newline
                : strippedContent;

            if (desiredContent == originalContent)
            {
                Debug.Log("[XMediator] Google Mobile Ads Next-Gen exclusions already up to date");
                return false;
            }

            File.WriteAllText(_gradleFilePath, desiredContent);
            Debug.Log(enabled
                ? "[XMediator] Applied Google Mobile Ads Next-Gen exclusions to mainTemplate.gradle"
                : "[XMediator] Removed Google Mobile Ads Next-Gen exclusions from mainTemplate.gradle");
            return true;
        }

        private static string ManagedBlock(string newline) =>
            StartMarker + newline +
            "configurations.configureEach {" + newline +
            "    exclude group: 'com.google.android.gms', module: 'play-services-ads'" + newline +
            "    exclude group: 'com.google.android.gms', module: 'play-services-ads-lite'" + newline +
            "}" + newline +
            EndMarker;

        private static bool TryRemoveManagedBlocks(string content, string newline, out string strippedContent)
        {
            strippedContent = content;
            while (true)
            {
                var startIndex = strippedContent.IndexOf(StartMarker, StringComparison.Ordinal);
                var endIndex = strippedContent.IndexOf(EndMarker, StringComparison.Ordinal);
                if (startIndex < 0 && endIndex < 0)
                {
                    return true;
                }
                if (startIndex < 0 || endIndex < startIndex)
                {
                    LogMalformedMarkers();
                    return false;
                }

                var nestedStart = strippedContent.IndexOf(StartMarker, startIndex + StartMarker.Length,
                    StringComparison.Ordinal);
                var endMarkerIndex = strippedContent.IndexOf(EndMarker, startIndex + StartMarker.Length,
                    StringComparison.Ordinal);
                if (endMarkerIndex < 0 || (nestedStart >= 0 && nestedStart < endMarkerIndex))
                {
                    LogMalformedMarkers();
                    return false;
                }

                var blockStart = startIndex;
                var blockEnd = endMarkerIndex + EndMarker.Length;
                if (blockStart >= 2 && strippedContent.Substring(blockStart - 2, 2) == "\r\n")
                {
                    blockStart -= 2;
                }
                else if (blockStart >= 1 && strippedContent[blockStart - 1] == '\n')
                {
                    blockStart -= 1;
                }
                if (blockEnd + 2 <= strippedContent.Length &&
                    strippedContent.Substring(blockEnd, 2) == "\r\n")
                {
                    blockEnd += 2;
                }
                else if (blockEnd < strippedContent.Length && strippedContent[blockEnd] == '\n')
                {
                    blockEnd += 1;
                }

                var prefix = strippedContent.Substring(0, blockStart);
                var suffix = strippedContent.Substring(blockEnd);
                var separator = prefix.Length > 0 && suffix.Length > 0 &&
                    !EndsWithLineBreak(prefix) && !StartsWithLineBreak(suffix)
                        ? newline
                        : string.Empty;
                strippedContent = prefix + separator + suffix;
            }
        }

        private static bool EndsWithLineBreak(string value) =>
            value.EndsWith("\n") || value.EndsWith("\r");

        private static bool StartsWithLineBreak(string value) =>
            value.StartsWith("\n") || value.StartsWith("\r");

        private static void LogMalformedMarkers() =>
            Debug.LogWarning("[XMediator] Malformed Google Mobile Ads Next-Gen exclusions markers " +
                "in mainTemplate.gradle; leaving the file unchanged");
    }
}
