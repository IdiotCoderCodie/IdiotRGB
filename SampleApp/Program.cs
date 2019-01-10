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

            Console.WriteLine("Press Any Key to Exit...");
            Console.ReadKey();
        }
    }
}
