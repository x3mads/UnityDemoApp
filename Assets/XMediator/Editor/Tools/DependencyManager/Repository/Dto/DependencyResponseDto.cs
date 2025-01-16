using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.DependencyManager.Repository.Dto
{
    [Serializable]
    internal class DependencyResponseDto
    {
        public string name;
        public string description;
        public DependencyVersionDto[] versions;

        public AvailableVersion ToModel()
        {
            var preferredVersion = versions.FirstOrDefault(version => version.preferred);
            string[] nonCompliantRules = null;
            if (preferredVersion?.non_compliant_rules != null)
            {
                nonCompliantRules = Array.ConvertAll(preferredVersion?.non_compliant_rules,
                    NonCompliantRulesToString);
            }
            return new AvailableVersion(
                name: name,
                description: description,
                version: new SemanticVersion(preferredVersion?.version),
                androidVersion: preferredVersion?.android_version,
                iOSVersion: preferredVersion?.ios_version,
                downloadUrl: preferredVersion?.download_url,
                downloadFilename: preferredVersion?.download_filename,
                isCompliant: preferredVersion?.is_compliant ?? true,
                nonCompliantRules: nonCompliantRules
            );
        }

        public AvailableDependency ToDependencyModel()
        {
            List<AvailableVersion> availableVersions = new List<AvailableVersion>();
            foreach (var dependencyVersionDto in versions)
            {
                string[] nonCompliantRules = null;
                if (dependencyVersionDto?.non_compliant_rules != null)
                {
                    nonCompliantRules = Array.ConvertAll(dependencyVersionDto?.non_compliant_rules,
                        NonCompliantRulesToString);
                }
                var version = new AvailableVersion(
                    name: name,
                    description: description,
                    version: new SemanticVersion(dependencyVersionDto.version),
                    androidVersion: dependencyVersionDto.android_version,
                    iOSVersion: dependencyVersionDto.ios_version,
                    downloadUrl: dependencyVersionDto.download_url,
                    downloadFilename: dependencyVersionDto.download_filename,
                    isCompliant: dependencyVersionDto.is_compliant,
                    nonCompliantRules: nonCompliantRules
                );
                availableVersions.Add(version);
            }
            return new AvailableDependency(name: name, description: description, availableVersions.ToArray());
        }

        public static string NonCompliantRulesToString(DependencyRuleDto dependencyRuleDto)
        {
            return $" - {dependencyRuleDto.dependency} ({dependencyRuleDto.version_type}) from {dependencyRuleDto.from} to {dependencyRuleDto.to}";
        }
    }
}