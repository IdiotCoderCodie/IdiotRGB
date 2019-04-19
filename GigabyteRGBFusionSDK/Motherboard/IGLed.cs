using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GigabyteRGBFusionSDK.Motherboard
{
  /// <summary>
  /// IGLed is the interface defining API to communicate with the RGB Fusion Motherboard API.
  /// </summary>
  public abstract class IGLed
  {
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
    /// <returns>True on success. False otherwise.</returns>
    public abstract bool GetLedLayout(List<GLedType> ledLayout);

    /// <summary>
    /// Writes settings into the MCU. Note: The settings will not apply until <c>Apply</c> is called.
    /// </summary>
    /// <param name="settingData">Array of LedSetting structures to write into the MCU.</param>
    /// <returns>On SUCCESS: 0.</returns>
    public abstract uint SetLedData(GLedSetting[] settingData);

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
