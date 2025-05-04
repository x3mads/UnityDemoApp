#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using XMediator.Editor.Tools.MetaMediation.Repository;

namespace XMediator.Editor.Android
{
    public class AndroidResolverPreprocessor : IPreprocessBuildWithReport
    {
        private const string XMediatorDependencyName = "com.etermax.android.xmediator:unity-proxy";
        private const string XMediatorX3MDependencyName = "com.x3mads.android.xmediator:unity-proxy";
        private const string VersionPattern = "spec=\"" + XMediatorDependencyName + ":(.*?)\"";
        private const string VersionX3MPattern = "spec=\"" + XMediatorX3MDependencyName + ":(.*?)\"";

        private static readonly string MainTemplatePath = Path.Combine("Assets/Plugins/Android", "mainTemplate.gradle");

        private static readonly string XMediatorDependenciesPath =
            Path.Combine(Application.dataPath, "XMediatorSettings", "Editor", "Main","XMediatorDependencies.xml");

        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!MainTemplateExists())
            {
                throw new BuildFailedException(
                    "Be sure to provide a custom mainTemplate.gradle file.\n" +
                    "(File -> Build Settings -> Android -> Player Settings... -> Player -> Publishing Settings -> Custom Main Gradle Template)"
                );
            }

            if (!DidRunAndroidResolver())
            {
                throw new BuildFailedException(
                    "Error: You must resolve Android dependencies. Please run\n" +
                    "\t\tAssets -> External Dependency Manager -> Android Resolver -> Resolve\n" +
                    "and try again."
                );
            }
            
            // Exclude dependencies
            var excludeList = ExcludeDependencyManager.LoadExcludeDependencies();
            if (excludeList.Count <= 0) return;
            
            var excluder = new GradleDependencyExcluder(MainTemplatePath);
            excluder.ApplyExclusions(excludeList);
        }

        private bool MainTemplateExists() => File.Exists(MainTemplatePath);

        private bool DidRunAndroidResolver()
        {
            var mainTemplateFile = File.ReadAllText(MainTemplatePath);
            var requiredDependency = GetRequiredDependency();
            var requiredX3MDependency = GetRequiredX3MDependency();

            return mainTemplateFile.Contains(requiredDependency) || mainTemplateFile.Contains(requiredX3MDependency);
        }

        private string GetRequiredDependency()
        {
            var xMediatorDependencies = File.ReadAllText(XMediatorDependenciesPath);
            var match = Regex.Match(xMediatorDependencies, VersionPattern, RegexOptions.Singleline);
            var requiredVersion =  match.Groups[1].Value;
            return $"{XMediatorDependencyName}:{requiredVersion}";
        }
        
        private string GetRequiredX3MDependency()
        {
            var xMediatorDependencies = File.ReadAllText(XMediatorDependenciesPath);
            var match = Regex.Match(xMediatorDependencies, VersionX3MPattern, RegexOptions.Singleline);
            var requiredVersion =  match.Groups[1].Value;
            return $"{XMediatorX3MDependencyName}:{requiredVersion}";
        }
    }
}

#endif