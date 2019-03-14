using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GigabyteRGBFusionSDK;
namespace SampleApp
{
  class Program
  {
    static void MotherboardControlAPI()
    {
      IGLed gLed = new GLedImpl();

      string rgbFusionSDKVer = "";
      gLed.GetSdkVersion(out rgbFusionSDKVer);
      Console.WriteLine("Gigabyte Fusion RGB SDK Version " + rgbFusionSDKVer + " Detected.");
      Console.WriteLine("Initializing...");

      uint result = gLed.InitAPI();
      if (result != 0u)
      {
        Console.WriteLine("Failed to Intialize RGB Fusion SDK");
        return;
      }

      Console.WriteLine("Successfully Initialized RGB Fusion SDK");
      string firmwareStr = gLed.FirmwareVersion();
      Console.WriteLine("RGB Fusion MCU Firmware Version: " + firmwareStr);

      int maxDivision = gLed.GetMaxDivision();
      Console.WriteLine("Max LED zones supported: " + maxDivision);

      byte[] ledLayout = new byte[maxDivision];
      result = gLed.GetLedLayout(ledLayout, ledLayout.Length);

      var li = 0;
      foreach (var led in ledLayout)
      {
        IGLed.LedType ledType = (IGLed.LedType)led;
        Console.WriteLine("Zone " + li + " Led: " + ledType.ToString());
        ++li;
      }

      IGLed.LedSetting[] nullAllSettings = new IGLed.LedSetting[maxDivision];
      for (var i = 0; i < nullAllSettings.Length; i++)
      {
        nullAllSettings[i].m_LedMode = 0;
      }

      IGLed.LedSetting[] ledSettings = new IGLed.LedSetting[maxDivision];

      for (var i = 0; i < ledSettings.Length; i++)
      {
        ledSettings[i].m_LedMode = (byte)IGLed.LedMode.Flash;
        ledSettings[i].m_MaxBrightness = 100;
        ledSettings[i].m_MinBrightness = 0;
        ledSettings[i].m_Colour = 0xFFFFFFFF;
        ledSettings[i].m_Time0 = 500;
        ledSettings[i].m_Time1 = 700;
        ledSettings[i].m_Time2 = 700 * 50;
        ledSettings[i].m_CtrlVal0 = 50;
      }

      result = gLed.SetLedData(nullAllSettings);
      result = gLed.Apply(-1);

      Console.WriteLine();
      Console.WriteLine("IDENTIFYING ZONES");
      for (var i = 0; i < maxDivision; i++)
      {
        result = gLed.SetLedData(ledSettings);
        int applyBit = (int)Math.Pow(2, i);
        result = gLed.Apply(applyBit);
        Console.WriteLine("Zone " + i + " by Flashing White.");
        Console.WriteLine("Press ANY KEY to move to next zone.\n");
        Console.ReadKey();
        result = gLed.SetLedData(nullAllSettings);
        result = gLed.Apply(-1);
      }
    }

    static void PeripheralsAPI()
    {
      // Trying to get VGA & Peripherals LED Control API working (GvLedLib.dll)
      IGvLed peripherals = new GvLedImpl();

      int deviceCount = 0;
      int[] deviceArray = new int[128];
      try
      {
        peripherals.Initialize(out deviceCount, deviceArray);
        int major = 0, minor = 0;
        peripherals.GetVersion(out major, out minor);

        Console.WriteLine("GvLedLib.dll Version: " + major + "." + minor);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return;
      }

      bool vgafound = false;
      Console.WriteLine("{0} VGA/Peripherals found", deviceCount);
      for (int i = 0; i < deviceCount; ++i)
      {
        IGvLed.GVLED_DeviceID id = (IGvLed.GVLED_DeviceID)deviceArray[i];
        Console.WriteLine("Device #{0}: ", id.ToString());

        if (id == IGvLed.GVLED_DeviceID.GVLED_VGA)
          vgafound = true;
      }

      if (vgafound)
      {
        string str;
        // Cannot get GetVgaModelName working correctly. Will have
        // to try a simple C app to see if that works before putting
        // anymore effort into it here. =/
        uint ret = peripherals.GetVgaModelName(out str);
        Console.WriteLine("VGA Name: {0}", str);
      }

      IGvLed.GVLED_CFG config = new IGvLed.GVLED_CFG();
      config.on = true;
      config.color = 0x000000FF;
      config.type = IGvLed.GVLED_LEDType.Pulsing;
      config.maxBright = 10;
      config.minBright = 10;
      peripherals.Save(-1, config);
    }

    static void Main(string[] args)
    {
      PeripheralsAPI();

      //MotherboardControlAPI();
    }
  }
}
