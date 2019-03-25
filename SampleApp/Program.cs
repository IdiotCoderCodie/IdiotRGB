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
using System.Threading;

namespace SampleApp
{
  class Program
  {
    static void ApplyGLedConfigFromFile(string path, IGLed gLed, int ledCount)
    {
      XmlSerializer xmlSerializer = new XmlSerializer(typeof(GLedSetting));

      FileStream input = File.OpenRead(path);

      try
      {
        GLedSetting ledSetting = (GLedSetting)xmlSerializer.Deserialize(input);

        GLedSetting[] array = new GLedSetting[ledCount];
        for (int i = 0; i < ledCount; i++)
        {
          array[i] = ledSetting;
        }

        gLed.SetLedData(array);
        gLed.Apply(-1); // I don't think this will work? Unsure but I think we may need "maxDivision" # of GLedSettings.
      }
      catch (Exception e)
      {
        Console.WriteLine("Invalid input file: " + path);
        Console.WriteLine("ERROR: " + e.Message);
      }

      input.Close();
    }

    static volatile bool moboThreadStop = false;
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
      result = gLed.SetLedData(nullAllSettings);
      result = gLed.Apply(-1);

      string moboXmlDirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//IdiotRGB//RGBFusion/Motherboard";
      if (!Directory.Exists(moboXmlDirPath))
      {
        var info = Directory.CreateDirectory(moboXmlDirPath);
      }

      string moboXmlFilePath = moboXmlDirPath + "//GLedType_SampleTest.xml";

      GLedSetting loadedLedSetting = new GLedSetting();
      XmlSerializer xmlSerializer = new XmlSerializer(typeof(GLedSetting));

      if (!File.Exists(moboXmlFilePath))
      {
        FileStream output = File.Create(moboXmlFilePath);

        // Initialize the file with some default GLedSetting
        loadedLedSetting.m_Colour = new GLedColour(0, 255, 0, 0); // TODO: Check what the format of this is. WWRRGGBB??
        loadedLedSetting.m_LedMode = GLedMode.Static;
        loadedLedSetting.m_MaxBrightness = 100;

        xmlSerializer.Serialize(output, loadedLedSetting);
        output.Close();
      }

      ApplyGLedConfigFromFile(moboXmlFilePath, gLed, maxDivision);

      using (FileSystemWatcher watcher = new FileSystemWatcher())
      {
        watcher.Path = moboXmlDirPath;
        watcher.Filter = Path.GetFileName(moboXmlFilePath);
        watcher.NotifyFilter = NotifyFilters.LastWrite
                             | NotifyFilters.LastAccess
                             | NotifyFilters.Size;
        watcher.Changed += (object source, FileSystemEventArgs e) =>
        {
          ApplyGLedConfigFromFile(e.FullPath, gLed, maxDivision);
        };

        Console.WriteLine("Watching file: " + moboXmlFilePath);
        watcher.EnableRaisingEvents = true;

        while (!moboThreadStop)
        {
          Thread.Sleep(5);
        }
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

    static volatile bool periphThreadStop = false;
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
        config.color = new GvLedColour(0, 0, 255, 0);
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

      using (FileSystemWatcher watcher = new FileSystemWatcher())
      {
        watcher.Path = Path.GetDirectoryName(path);
        watcher.Filter = Path.GetFileName(path);
        watcher.NotifyFilter = NotifyFilters.LastWrite // Looks like only Size is working =/
                             | NotifyFilters.LastAccess
                             | NotifyFilters.Size;

        watcher.Changed += (object source, FileSystemEventArgs e) =>
        {
          ApplyConfigFromFile(e.FullPath, peripherals);
        };

        Console.WriteLine("Watching file: " + path);
        watcher.EnableRaisingEvents = true; // Start watching.

        while (!periphThreadStop)
        {
          Thread.Sleep(5);
        }
      }
    }

    static void Main(string[] args)
    {
      ThreadStart periphThreadRef = new ThreadStart(PeripheralsAPI);
      Thread periphThread = new Thread(periphThreadRef);
      periphThread.Start();

      ThreadStart moboThreadRef = new ThreadStart(MotherboardControlAPI);
      Thread moboThread = new Thread(moboThreadRef);
      moboThread.Start();

      Console.WriteLine("Press Any Key to quit.");
      Console.ReadKey();

      periphThreadStop = true;
      moboThreadStop = true;

      periphThread.Join();
      moboThread.Join();
    }
  }
}
