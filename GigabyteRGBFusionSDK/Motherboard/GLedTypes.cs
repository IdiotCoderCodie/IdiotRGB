using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GigabyteRGBFusionSDK.Motherboard
{
  [Serializable()]
  [StructLayout(LayoutKind.Sequential)]
  public struct GLedColour
  {
    public GLedColour(byte w, byte r, byte g, byte b)
    {
      m_ww = w;
      m_rr = r;
      m_gg = g;
      m_bb = b;
    }

    public GLedColour(byte v)
      : this(v, v, v, v)
    {  
    }

    public byte m_bb;
    public byte m_gg;
    public byte m_rr;
    public byte m_ww;
  }

  /// <summary>
  /// The structure used to set motherboard LED mode, colour, timing, and other mode-dependent settings.
  /// </summary>
  [Serializable()]
  [StructLayout(LayoutKind.Sequential)]
  public struct GLedSetting
  {
    public byte       m_Reserve0;
    public GLedMode   m_LedMode;
    public byte       m_MaxBrightness;  // [0 - 100]
    public byte       m_MinBrightness;  // [0 - 100]
    public GLedColour m_Colour;
    public ushort     m_Time0;          // [ms : 0 - 65535]
    public ushort     m_Time1;          // [ms : 0 - 65535]
    public ushort     m_Time2;          // [ms : 0 - 65535]
    public byte       m_CtrlVal0;       // [0 - 255]
    public byte       m_CtrlVal1;       // [0 - 255]
  }

  /// <summary>
  /// Defines the mode values used to set <c>LedSetting::m_LedMode</c>.
  /// </summary>
  /// <remarks>DigitalModeX values are reserved for DigitalLED1 and DigitalLED2 LedTypes
  /// </remarks>
  public enum GLedMode : byte
  {
    Null          = 0,
    Pulse         = 1,
    Music         = 2,
    ColorCycle    = 3,
    Static        = 4,
    Flash         = 5,
    Transition    = 8,
    DigitalModeA  = 10,
    DigitalModeB  = 11,
    DigitalModeC  = 12,
    DigitalModeD  = 13,
    DigitalModeE  = 14,
    DigitalModeF  = 15,
    DigitalModeG  = 16,
    DigitalModeH  = 17,
    DigitalModeI  = 18
  }

  /// <summary>
  /// Defines the Led type returned by GetLedLayout().
  /// </summary>
  /// <remarks>
  /// DigitalLED1 supports DigitalModes: A, B, E
  /// DigitalLED2 supports DigitalModes: A, B, C, D, F, G, H, I
  /// </remarks>
  public enum GLedType
  {
    NoLED       = 0,
    AnalogLED   = 1,
    DigitalLED1 = 2, // Audio Shield
    DigitalLED2 = 3  // LED Strip
  }
}
