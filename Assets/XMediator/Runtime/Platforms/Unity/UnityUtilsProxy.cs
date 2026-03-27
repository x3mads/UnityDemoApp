using UnityEngine;

namespace XMediator.Unity
{
    internal class UnityUtilsProxy : UtilsProxy
    {
        public float GetScreenDensity()
        {
            return 1f;
        }

        public bool IsTablet()
        {
            return false;
        }
    }
}
