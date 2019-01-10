using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GigabyteRGBFusionSDKWrapper
{
    public class RGBFusionSDK
    {
        [DllImport("GLedApi ", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern ulong dllexp_GetSdkVersion(StringBuilder lpReturnBuf, int bufLen);
        public static ulong GetSdkVersion(out String returnStr)
        {
            StringBuilder strBuilder = new StringBuilder(16);
            ulong output = dllexp_GetSdkVersion(strBuilder, strBuilder.Capacity);
            returnStr = strBuilder.ToString();
            return output;
        }
    }
}
