namespace XMediator.Editor.Tools.MetaMediation.Entities
{
    internal class Pod
    {
        public string Name { get; }
        public string Version { get; }

        public Pod(string name, string version)
        {
            Name = name;
            Version = version;
        }
    }
}