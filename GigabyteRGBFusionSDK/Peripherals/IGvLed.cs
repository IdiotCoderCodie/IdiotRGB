using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GigabyteRGBFusionSDK
{
  public abstract class IGvLed
  {
    public enum GVLED_DeviceID
    {
      GVLED_VGA = 0x1001,
      GVLED_XK700_KEYB = 0x2001,
      GVLED_AORUS_K7_KEYB = 0x2002,
      GVLED_AORUS_K9_KEYB = 0x2003,
      GVLED_XM300_MOUSE = 0x3001,
      GVLED_AORUS_M3_MOUSE = 0x3002,
      GVLED_1070IXEB_GAMING_VGA_BOX = 0x4001,
      GVLED_1080IXEB_GAMING_VGA_BOX = 0x4002,
      GVLED_XTV700_CASE = 0x4003,
      GVLED_XC300W_CASE = 0x4004,
      GVLED_XC700W_CASE = 0x4005,
      GVLED_XH300_EARPHONE = 0x4006,
      GVLED_AORUS_H5_EARPHONE = 0x4007,
      GVLED_AC300W_CASE = 0x4008,
      GVLED_ATC700_CPU_COOLER = 0x4009,
      GVLED_0x400A_AORUS_P7_MOUSEPAD = 0x400A,
      GVLED_MOTHERBOARD = 0x5001
    }

    public enum GVLED_LEDType
    {
      Consistent = 1,
      Pulsing = 2,
      GingleFlash = 3,
      DualFlash = 4,
      Cycling = 5,
      Ripple = 6,
      Reactive = 7,
      Wave = 8,
      Running = 9,
      RealTime = 20
    }

    public struct GVLED_CFG
    {
      public GVLED_LEDType type;
      public int speed;          // 1 - 10
      public uint time1;         // ms
      public uint time2;         // ms
      public uint time3;         // ms
      public int minBright;      // 1 - 10
      public int maxBright;      // 1 - 10
      public uint color;         // 0x00RRGGBB
      public int angle;          // 1 - 360
      public bool on;
      public bool sync;
    }

    public abstract uint Initialize(out int deviceCount, int[] deviceArray);

    public abstract uint GetVersion(out int majorVersion, out int minorVersion);

    public abstract uint Save(int index, GVLED_CFG config);

    public abstract uint Set(int index, GVLED_CFG config);

    public abstract uint GetVgaModelName(out string array);
  }
}
