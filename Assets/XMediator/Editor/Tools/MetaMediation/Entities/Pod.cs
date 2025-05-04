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

        public override bool Equals(object obj)
        {
            if (obj is Pod other)
            {
                return Name == other.Name && Version == other.Version;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode() * 397) ^ Version.GetHashCode();
            }
        }
    }
}