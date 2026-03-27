namespace XMediator.Api
{
    /// <summary>
    /// Provides utility methods for retrieving native device information.
    /// </summary>
    public class Utils
    {
        private readonly UtilsProxy _utilsProxy;

        internal Utils()
        {
            _utilsProxy = ProxyFactory.CreateInstance<UtilsProxy>("UtilsProxy");
        }

        /// <summary>
        /// Returns the native screen density (scale factor).
        /// On iOS, this is <c>UIScreen.main.scale</c> (e.g. 2.0 or 3.0).
        /// On Android, this is <c>DisplayMetrics.density</c>.
        /// In the Unity editor, this returns 1.0.
        /// </summary>
        /// <returns>The native screen density as a float.</returns>
        public float GetScreenDensity()
        {
            return _utilsProxy.GetScreenDensity();
        }

        /// <summary>
        /// Returns true if the device is a tablet (i.e. iPad).
        /// In the Unity editor, this returns false.
        /// </summary>
        /// <returns>True if the current device is a tablet.</returns>
        public bool IsTablet()
        {
            return _utilsProxy.IsTablet();
        }
    }
}
