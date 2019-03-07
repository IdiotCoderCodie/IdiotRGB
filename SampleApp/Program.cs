using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GigabyteRGBFusionSDKWrapper;
namespace SampleApp
{
  class Program
  {
    static void MotherboardControlAPI()
    {
      string rgbFusionSDKVer = "";
      RGBFusionSDK.GetSdkVersion(out rgbFusionSDKVer);
      Console.WriteLine("Gigabyte Fusion RGB SDK Version " + rgbFusionSDKVer + " Detected.");
      Console.WriteLine("Initializing...");

      uint result = RGBFusionSDK.InitAPI();

      if (result != 0u)
      {
        Console.WriteLine("Failed to Intialize RGB Fusion SDK");
        return;
      }

      Console.WriteLine("Successfully Initialized RGB Fusion SDK");
      string firmwareStr = RGBFusionSDK.FirmwareVersion();
      Console.WriteLine("RGB Fusion MCU Firmware Version: " + firmwareStr);

      int maxDivision = RGBFusionSDK.GetMaxDivision();
      Console.WriteLine("Max LED zones supported: " + maxDivision);

      byte[] ledLayout = new byte[maxDivision];
      result = RGBFusionSDK.GetLedLayout(ledLayout, ledLayout.Length);

      var li = 0;
      foreach (var led in ledLayout)
      {
        RGBFusionSDK.LedType ledType = (RGBFusionSDK.LedType)led;
        Console.WriteLine("Zone " + li + " Led: " + ledType.ToString());
        ++li;
      }

      RGBFusionSDK.LedSetting[] nullAllSettings = new RGBFusionSDK.LedSetting[maxDivision];
      for (var i = 0; i < nullAllSettings.Length; i++)
      {
        nullAllSettings[i].m_LedMode = 0;
      }

      RGBFusionSDK.LedSetting[] ledSettings = new RGBFusionSDK.LedSetting[maxDivision];

      for (var i = 0; i < ledSettings.Length; i++)
      {
        ledSettings[i].m_LedMode = (byte)RGBFusionSDK.LedMode.Flash;
        ledSettings[i].m_MaxBrightness = 100;
        ledSettings[i].m_MinBrightness = 0;
        ledSettings[i].m_Colour = 0xFFFFFFFF;
        ledSettings[i].m_Time0 = 500;
        ledSettings[i].m_Time1 = 700;
        ledSettings[i].m_Time2 = 700 * 50;
        ledSettings[i].m_CtrlVal0 = 50;
      }

      result = RGBFusionSDK.SetLedData(nullAllSettings);
      result = RGBFusionSDK.Apply(-1);

      Console.WriteLine();
      Console.WriteLine("IDENTIFYING ZONES");
      for (var i = 0; i < maxDivision; i++)
      {
        result = RGBFusionSDK.SetLedData(ledSettings);
        int applyBit = (int)Math.Pow(2, i);
        result = RGBFusionSDK.Apply(applyBit);
        Console.WriteLine("Zone " + i + " by Flashing White.");
        Console.WriteLine("Press ANY KEY to move to next zone.\n");
        Console.ReadKey();
        result = RGBFusionSDK.SetLedData(nullAllSettings);
        result = RGBFusionSDK.Apply(-1);
      }
    }

    static void Main(string[] args)
    {
      // Trying to get VGA & Peripherals LED Control API working (GvLedLib.dll)
      int deviceCount = 0;
      int[] deviceArray = new int[128];
      try
      {
        RGBFusionSDK.GvLedInitialize(out deviceCount, deviceArray);
        int major = 0, minor = 0;
        RGBFusionSDK.GvLedGetSDKVersion(out major, out minor);

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
        RGBFusionSDK.GVLED_DeviceID id = (RGBFusionSDK.GVLED_DeviceID)deviceArray[i];
        Console.WriteLine("Device #{0}: ", id.ToString());

        if (id == RGBFusionSDK.GVLED_DeviceID.GVLED_VGA)
          vgafound = true;
      }

      if (vgafound)
      {
        // Not working correctly?
        byte[] vgaNameByteArray = new byte[256];
        RGBFusionSDK.GvLedGetVgaModelName(out vgaNameByteArray);
        string converted = Encoding.UTF8.GetString(vgaNameByteArray, 0, vgaNameByteArray.Length);
        Console.WriteLine("VGA Name: {0}", converted);
      }

      RGBFusionSDK.GVLED_CFG config = new RGBFusionSDK.GVLED_CFG();
      config.on = true;
      config.color = 0x000000FF;
      config.type = RGBFusionSDK.GVLED_LEDType.Pulsing;
      config.maxBright = 10;
      config.minBright = 10;
      RGBFusionSDK.GvLedSave(-1, config);
    }
  }
}
