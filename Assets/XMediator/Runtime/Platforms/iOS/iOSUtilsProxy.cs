using System.Runtime.InteropServices;

namespace XMediator.iOS
{
    internal class iOSUtilsProxy : UtilsProxy
    {
        public float GetScreenDensity()
        {
            return X3MGetScreenScale();
        }

        public bool IsTablet()
        {
            return X3MIsTablet();
        }

        [DllImport("__Internal")]
        private static extern float X3MGetScreenScale();

        [DllImport("__Internal")]
        private static extern bool X3MIsTablet();
    }
}
