namespace XMediator.Core.Util
{
    internal class XMediatorMainThreadDispatcherFactory
    {
        internal static bool IsTest = false;

        internal static void Create()
        {
            if (!IsTest)
            {
                XMediatorMainThreadDispatcher.Initialize();
            }
        }
    }
}