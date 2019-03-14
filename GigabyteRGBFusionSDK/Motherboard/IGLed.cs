using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GigabyteRGBFusionSDK
{
  /// <summary>
  /// IGLed is the interface defining API to communicate with the RGB Fusion Motherboard API.
  /// </summary>
  public abstract class IGLed
  {

    // TODO: Move types to separate file?
    /// <summary>
    /// The structure used to set mother LED mode, colour, timing, and other mode-dependent settings.
    /// </summary>
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

    /// <summary>
    /// Defines the mode values used to set <c>LedSetting::m_LedMode</c>.
    /// </summary>
    /// <remarks>DigitalModeX values are reserved for DigitalLED1 and DigitalLED2 LedTypes
    /// </remarks>
    public enum LedMode
    {
      Null = 0,
      Pulse = 1,
      Music = 2,
      ColorCycle = 3,
      Static = 4,
      Flash = 5,
      Transition = 8,
      DigitalModeA = 10,
      DigitalModeB = 11,
      DigitalModeC = 12,
      DigitalModeD = 13,
      DigitalModeE = 14,
      DigitalModeF = 15,
      DigitalModeG = 16,
      DigitalModeH = 17,
      DigitalModeI = 18
    }

    /// <summary>
    /// Defines the Led type returned by GetLedLayout().
    /// </summary>
    /// <remarks>
    /// DigitalLED1 supports DigitalModes: A, B, E
    /// DigitalLED2 supports DigitalModes: A, B, C, D, F, G, H, I
    /// </remarks>
    public enum LedType
    {
      NoLED = 0,
      AnalogLED = 1,
      DigitalLED1 = 2, // Audio Shield
      DigitalLED2 // LED Strip
    }

    /// <summary>
    /// Retrieves the revision number of the RGB Fusion SDK currently being used and stored it in returnStr.
    /// </summary>
    /// <param name="returnStr">Output parameter which will hold the SDK revision number if function is successful.</param>
    /// <returns>Returns 0 on success.</returns>
    public abstract ulong GetSdkVersion(out String returnStr);

    /// <summary>
    /// Initializes the SDK library for the current application. Must be called before all other functions other than <c>GetSDKVersion()</c>.
    /// </summary>
    /// <returns>On SUCCESS: 0.</returns>
    public abstract uint InitAPI();

    /// <summary>
    /// Gets the maximum number of LED zones that the MCU currently supports.
    /// </summary>
    /// <returns>On SUCCESS: number of zones. On FAIL: -1.</returns>
    public abstract int GetMaxDivision();

    /// <summary>
    /// Returns the current LED layout on the motherboard.
    /// </summary>
    /// <param name="array">
    /// Pointer to a byte array that receives the LED layout on the motherboard.
    /// </param>
    /// <param name="arraySize"></param>
    /// <returns>On SUCCESS: 0. Else 0x10DD : FAIL, 0x7A : INSUFFICIENT BUFFER, 0x32 NOT_SUPPORTED</returns>
    public abstract uint GetLedLayout(byte[] array, int arraySize);

    /// <summary>
    /// Writes settings into the MCU. Note: The settings will not apply until <c>Apply</c> is called.
    /// </summary>
    /// <param name="settingData">Array of LedSetting structures to write into the MCU.</param>
    /// <returns>On SUCCESS: 0.</returns>
    public abstract uint SetLedData(LedSetting[] settingData);

    /// <summary>
    /// Applies settings to the MCU that have been set through <c>SetLedData</c>.
    /// </summary>
    /// <param name="applyBit">Bitset used to control which LED zone(s) to apply the new settings.</param>
    /// <returns>On SUCCESS: 0.</returns>
    public abstract uint Apply(int applyBit);

    /// <summary>
    /// Begins to function after the MCU enters Music mode. Programmers must retrieve music signals themselves and then
    /// use this API to set the LEDs to turn on or off with music signals.
    /// </summary>
    /// <param name="iCtrl"></param>
    /// <returns></returns>
    public abstract uint BeatInput(int iCtrl);

    /// <summary>
    /// Calling this API is equivalent to sending the last commands (including mode, speed, brightness) to the MCu again.
    /// </summary>
    /// <returns>On SUCCESS: 0.</returns>
    public abstract uint Reset();

    /// <summary>
    /// Call this API to get the string of the MCU firmware version.
    /// </summary>
    /// <returns>The MCU firmware version as a string.</returns>
    public abstract string FirmwareVersion();
  }
}
