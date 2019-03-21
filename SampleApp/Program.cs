using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GigabyteRGBFusionSDK;
using GigabyteRGBFusionSDK.Motherboard;
using GigabyteRGBFusionSDK.Peripherals;
using System.IO;
using System.Xml.Serialization;
using System.Security.Permissions;

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
        GLedType ledType = (GLedType)led;
        Console.WriteLine("Zone " + li + " Led: " + ledType.ToString());
        ++li;
      }

      GLedSetting[] nullAllSettings = new GLedSetting[maxDivision];
      for (var i = 0; i < nullAllSettings.Length; i++)
      {
        nullAllSettings[i].m_LedMode = 0;
      }

      GLedSetting[] ledSettings = new GLedSetting[maxDivision];

      for (var i = 0; i < ledSettings.Length; i++)
      {
        ledSettings[i].m_LedMode = (byte)GLedMode.Flash;
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

    static void ApplyConfigFromFile(string path, IGvLed gvLed)
    {
      XmlSerializer xmlSerializer = new XmlSerializer(typeof(GvLedConfig));

      FileStream inFile = File.OpenRead(path);

      try
      {
        GvLedConfig config = (GvLedConfig)xmlSerializer.Deserialize(inFile);
        Console.WriteLine("Applying Config to VGA...");
        gvLed.Save(-1, config);
      }
      catch(Exception e)
      {
        Console.WriteLine("{0} was Invalid", path);
      }

      inFile.Close();
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
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
        GvDeviceID id = (GvDeviceID)deviceArray[i];
        Console.WriteLine("Device #{0}: ", id.ToString());

        if (id == GvDeviceID.GVLED_VGA)
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

      // TODO: Following could fail in multiple places, so make it safe.
      GvLedConfig config;
      XmlSerializer xmlSerializer = new XmlSerializer(typeof(GvLedConfig));

      string outputDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//IdiotRGB//RGBFusion//Peripherals";
      if (!Directory.Exists(outputDir))
      {
        var info = Directory.CreateDirectory(outputDir);
      }

      var path = outputDir + "//GvLedType_SampleTest.xml";
      if(!File.Exists(path))
      {
        config = new GvLedConfig();
        config.on = true;
        config.color = 0x0000FF00;
        config.type = GvLedType.Pulsing;
        config.maxBright = 10;
        config.minBright = 10;
        config.time1 = 100;
        config.time2 = 200;

        FileStream outFile = File.Create(path);
        xmlSerializer.Serialize(outFile, config);
        outFile.Close();
      }

      ApplyConfigFromFile(path, peripherals);

      // ToDo: Put in another thread.
      using (FileSystemWatcher watcher = new FileSystemWatcher())
      {
        watcher.Path = Path.GetDirectoryName(path);
        watcher.Filter = Path.GetFileName(path);
        watcher.NotifyFilter = NotifyFilters.LastWrite // Looks like only Size is working =/
                             | NotifyFilters.LastAccess
                             | NotifyFilters.Size;
                             //| NotifyFilters.FileName
                             //| NotifyFilters.DirectoryName

        watcher.Changed += (object source, FileSystemEventArgs e) =>
        {
          ApplyConfigFromFile(e.FullPath, peripherals);
        };

        Console.WriteLine("Watching file: " + path);
        watcher.EnableRaisingEvents = true; // Start watching.

        Console.WriteLine("Press Any Key to quit");
        Console.ReadKey();
      }
    }

    static void Main(string[] args)
    {
      PeripheralsAPI();
      //MotherboardControlAPI();
    }
  }
}
