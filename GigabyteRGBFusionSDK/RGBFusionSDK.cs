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
        public struct LedSetting
        {
            public byte m_Reserve0;
            public byte m_LedMode;
            public byte m_MaxBrightness;
            public byte m_MinBrightness;
            public uint m_Colour;
            public ushort m_Time0;
            public ushort m_Time1;
            public ushort m_Time2;
            public byte m_CtrlVal0;
            public byte m_CtrlVal1;
        }

        [DllImport("GLedApi ", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern uint dllexp_GetSdkVersion(StringBuilder lpReturnBuf, int bufLen);
        public static ulong GetSdkVersion(out String returnStr)
        {
            StringBuilder strBuilder = new StringBuilder(16);
            ulong output = dllexp_GetSdkVersion(strBuilder, strBuilder.Capacity);
            returnStr = strBuilder.ToString();
            return output;
        }

        [DllImport("GLedApi ", EntryPoint = "dllexp_InitAPI", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern uint InitAPI();

        [DllImport("GLedApi ", EntryPoint = "dllexp_GetMaxDivision", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int GetMaxDivision();

        [DllImport("GLedApi ", EntryPoint = "dllexp_GetLedLayout", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern uint GetLedLayout(byte[] array, int arraySize);

        [DllImport("GLedApi ", EntryPoint = "dllexp_SetLedData", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern uint dllexp_SetLedData(LedSetting[] setting, int arraySize);
        public static uint SetLedData(LedSetting[] settingData)
        {
            return dllexp_SetLedData(settingData, Marshal.SizeOf(settingData[0]) * settingData.Length);
        }

        [DllImport("GLedApi ", EntryPoint = "dllexp_Apply", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern uint Apply(int applyBit);


    }
}
