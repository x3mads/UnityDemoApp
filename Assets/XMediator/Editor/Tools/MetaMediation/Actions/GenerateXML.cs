using System;
using System.Xml.Linq;
using UnityEngine;
using XMediator.Editor.Tools.MetaMediation.Entities;
using XMediator.Editor.Tools.MetaMediation.Repository;

namespace XMediator.Editor.Tools.MetaMediation.Actions
{
    internal class GenerateXML
    {
        private DependenciesPreProcessor DependenciesPreProcessor { get; }

        internal GenerateXML(DependenciesPreProcessor preProcessor)
        {
            DependenciesPreProcessor = preProcessor;
        }
        
        public void Invoke(Dependencies dependencies, Action<XDocument> onSuccess, Action<Exception> onError)
        {
            try
            {
                var processedDependencies = DependenciesPreProcessor.Invoke(dependencies: dependencies);
                onSuccess.Invoke(UnityXMLGenerator.BuildUnityXMLSnippet(processedDependencies));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                onError.Invoke(e);
            }
        }
    }
}