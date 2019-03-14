using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GigabyteRGBFusionSDK
{
  /// <summary>
  /// Implements the IGLed interface, importing API from GLedLib.dll.
  /// </summary>
  public class GLedImpl : IGLed
  {
    [DllImport("GLedApi ", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern uint dllexp_GetSdkVersion(StringBuilder lpReturnBuf, int bufLen);
    public override ulong GetSdkVersion(out String returnStr)
    {
      StringBuilder strBuilder = new StringBuilder(16);
      ulong output = dllexp_GetSdkVersion(strBuilder, strBuilder.Capacity);
      returnStr = strBuilder.ToString();
      return output;
    }

    [DllImport("GLedApi ", EntryPoint = "dllexp_InitAPI", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern uint dllexp_InitAPI();
    public override uint InitAPI()
    {
      return dllexp_InitAPI();
    }

    [DllImport("GLedApi ", EntryPoint = "dllexp_GetMaxDivision", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int dllexp_GetMaxDivision();
    public override int GetMaxDivision()
    {
      return dllexp_GetMaxDivision();
    }

    [DllImport("GLedApi ", EntryPoint = "dllexp_GetLedLayout", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern uint dllexp_GetLedLayout(byte[] array, int arraySize);
    public override uint GetLedLayout(byte[] array, int arraySize)
    {
      return dllexp_GetLedLayout(array, arraySize);
    }

    [DllImport("GLedApi ", EntryPoint = "dllexp_SetLedData", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern uint dllexp_SetLedData(LedSetting[] setting, int arraySize);
    public override uint SetLedData(LedSetting[] settingData)
    {
      return dllexp_SetLedData(settingData, Marshal.SizeOf(settingData[0]) * settingData.Length);
    }

    [DllImport("GLedApi ", EntryPoint = "dllexp_Apply", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern uint dllexp_Apply(int applyBit);
    public override uint Apply(int applyBit)
    {
      return dllexp_Apply(applyBit);
    }

    [DllImport("GLedApi ", EntryPoint = "dllexp_BeatInput", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern uint dllexp_BeatInput(int iCtrl);
    public override uint BeatInput(int iCtrl)
    {
      return dllexp_BeatInput(iCtrl);
    }

    [DllImport("GLedApi ", EntryPoint = "dllexp_IT8295_Reset", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern uint dllexp_IT8295_Reset();
    public override uint Reset()
    {
      return dllexp_IT8295_Reset();
    }

    [DllImport("GLedApi ", EntryPoint = "dllexp_Get_IT8295_FwVer", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern uint dllexp_Get_IT8295_FirmwareVersion(byte[] array, int arraySize);
    public override string FirmwareVersion()
    {
      string returnStr = "";
      byte[] firmwareBytes = new byte[4];
      if (dllexp_Get_IT8295_FirmwareVersion(firmwareBytes, firmwareBytes.Length) == 0)
      {
        returnStr += firmwareBytes[3] + "." + firmwareBytes[2] + "." + firmwareBytes[1] + "." + firmwareBytes[0];
        return returnStr;
      }

      return returnStr;
    }
  }
}
