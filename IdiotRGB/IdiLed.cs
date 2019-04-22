using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdiotRGB
{
  public enum IdiLedMode
  {
    STATIC = 0,
    FLASH = 1
  }

  public class IdiLed
  {
    public bool Enabled { get; set; }

    public IdiLedColour Colour { get; set; }

    public IdiLedMode Mode { get; set; }

    public byte MinBrightness
    {
      get { return _minBrightness; }
      set { _minBrightness = Math.Min(Math.Max(value, (byte)0), (byte)100); } // [0 - 100]
    }

    public byte MaxBrightness
    {
      get { return _maxBrightness; }
      set { _maxBrightness = Math.Min(Math.Max(value, (byte)0), (byte)100); } // [0 - 100]
    }

    public ushort TimeMs0
    {
      get { return _timeMs0; }
      set { _timeMs0 = Math.Min(Math.Max(value, (ushort)0), (ushort)2000); } // [0 - 2000]
    }

    public ushort TimeMs1
    {
      get { return _timeMs1; }
      set { _timeMs1 = Math.Min(Math.Max(value, (ushort)0), (ushort)2000); } // [0 - 2000]
    }

    private byte _minBrightness = 0;
    private byte _maxBrightness = 100;
    private ushort _timeMs0 = 500;
    private ushort _timeMs1 = 500;
  }
}
