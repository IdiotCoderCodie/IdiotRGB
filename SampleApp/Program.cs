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
        static void Main(string[] args)
        {
            string rgbFusionSDKVer = "";
            RGBFusionSDK.GetSdkVersion(out rgbFusionSDKVer);
            Console.WriteLine("Gigabyte Fusion RGB SDK Version " + rgbFusionSDKVer + " Detected.");
            Console.WriteLine("Initializing...");

            uint result = RGBFusionSDK.InitAPI();

            if (result != 0u) {
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
            foreach(var led in ledLayout)
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

            for(var i = 0; i < ledSettings.Length; i++)
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
    }
}
