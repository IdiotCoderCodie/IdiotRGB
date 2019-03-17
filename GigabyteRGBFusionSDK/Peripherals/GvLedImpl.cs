using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GigabyteRGBFusionSDK.Peripherals
{
  public class GvLedImpl : IGvLed
  {
    [DllImport("GvLedLib.dll", EntryPoint = "dllexp_GvLedInitial", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern uint dllexp_GvLedInitial(out int deviceCount, int[] deviceArray);
    public override uint Initialize(out int deviceCount, int[] deviceArray)
    {
      return dllexp_GvLedInitial(out deviceCount, deviceArray);
    }

    [DllImport("GvLedLib.dll", EntryPoint = "dllexp_GvLedGetVersion", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern uint dllexp_GvLedGetVersion(out int majorVersion, out int minorVersion);
    public override uint GetVersion(out int majorVersion, out int minorVersion)
    {
      return dllexp_GvLedGetVersion(out majorVersion, out minorVersion);
    }

    [DllImport("GvLedLib.dll", EntryPoint = "dllexp_GvLedSave", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern uint dllexp_GvLedSave(int index, GvLedConfig config);
    public override uint Save(int index, GvLedConfig config)
    {
      return dllexp_GvLedSave(index, config);
    }

    [DllImport("GvLedLib.dll", EntryPoint = "dllexp_GvLedSet", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern uint dllexp_GvLedSet(int index, GvLedConfig config);
    public override uint Set(int index, GvLedConfig config)
    {
      return dllexp_GvLedSet(index, config);
    }

    [DllImport("GvLedLib.dll", EntryPoint = "dllexp_GvLedGetVgaModelName", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern uint dllexp_GvLedGetVgaModelName(IntPtr array);
    public override uint GetVgaModelName(out string array)
    {
      IntPtr ptr = IntPtr.Zero;
      uint retVal = dllexp_GvLedGetVgaModelName(ptr);
      array = Marshal.PtrToStringUni(ptr);
      return retVal;
    }
  }
}
