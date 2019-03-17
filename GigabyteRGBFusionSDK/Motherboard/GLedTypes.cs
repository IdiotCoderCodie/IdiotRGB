using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigabyteRGBFusionSDK.Motherboard
{
  /// <summary>
  /// The structure used to set motherboard LED mode, colour, timing, and other mode-dependent settings.
  /// </summary>
  public struct GLedSetting
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

  /// <summary>
  /// Defines the mode values used to set <c>LedSetting::m_LedMode</c>.
  /// </summary>
  /// <remarks>DigitalModeX values are reserved for DigitalLED1 and DigitalLED2 LedTypes
  /// </remarks>
  public enum GLedMode
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
