using System.Collections.Generic;

namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal class ProcessedDependencies
    {
        public HashSet<string> AndroidRepositories { get; }
        public HashSet<string> Artifacts { get; }
        public HashSet<string> IOSSources { get; }
        public HashSet<Pod> Pods { get; }
          

        public ProcessedDependencies(HashSet<string> androidRepositories, HashSet<string> artifacts, HashSet<string> iOSSources, HashSet<Pod> pods)
        {
            AndroidRepositories = androidRepositories;
            Artifacts = artifacts;
            IOSSources = iOSSources;
            Pods = pods;
        }
    }
}