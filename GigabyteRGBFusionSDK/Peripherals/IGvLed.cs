using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GigabyteRGBFusionSDK.Peripherals
{
  public abstract class IGvLed
  {
    public abstract uint Initialize(out int deviceCount, int[] deviceArray);

    public abstract uint GetVersion(out int majorVersion, out int minorVersion);

    public abstract uint Save(int index, GvLedConfig config);

    public abstract uint Set(int index, GvLedConfig config);

    public abstract uint GetVgaModelName(out string array);
  }
}
