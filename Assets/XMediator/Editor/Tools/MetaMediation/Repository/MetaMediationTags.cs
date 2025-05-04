namespace XMediator.Editor.Tools.MetaMediation.Repository
{
    internal class MetaMediationTags
    {
        internal readonly string IOS;
        internal readonly string Android;

        internal MetaMediationTags(string ios, string android)
        {
            IOS = ios ?? "";
            Android = android ?? "";
        }

        internal MetaMediationTags UpdatingIOS(string ios)
        {
            return new MetaMediationTags(ios, Android);
        }
    }
}