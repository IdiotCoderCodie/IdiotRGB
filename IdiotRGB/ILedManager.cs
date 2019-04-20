using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdiotRGB
{
  public struct IdiLedColour
  {
    public IdiLedColour(byte r, byte g, byte b, byte w)
    {
      Red = r;
      Green = g;
      Blue = b;
      White = w;
    }

    public byte Red;
    public byte Green;
    public byte Blue;
    public byte White;
  }

  public enum IdiLedMode
  {
    STATIC = 0,
    FLASH = 1
  }

  public struct IdiLedProperties
  {
    public uint TimeMs0
    {
      get { return _timeMs0; }
      set { _timeMs0 = Math.Min(Math.Max(value, 0), 2000); } // [0 - 2000]
    }

    public uint TimeMs1
    {
      get { return _timeMs1; }
      set { _timeMs1 = Math.Min(Math.Max(value, 0), 2000); } // [0 - 2000]
    }

    public ushort MinBrightness
    {
      get { return _minBrightness; }
      set { MinBrightness = Math.Min(Math.Max(value, (ushort)0), (ushort)100); } // [0 - 100]
    }

    public ushort MaxBrightness
    {
      get { return _maxBrightness; }
      set { MaxBrightness = Math.Min(Math.Max(value, (ushort)0), (ushort)100); } // [0 - 100]
    }

    public IdiLedProperties(uint time0, uint time1, ushort minBrightness, ushort maxBrightness)
    {
      _timeMs0 = time0;
      _timeMs1 = time1;
      _minBrightness = minBrightness;
      _maxBrightness = maxBrightness;
    }

    private uint _timeMs0;
    private uint _timeMs1;
    private ushort _minBrightness;
    private ushort _maxBrightness;
  }

  public class IdiLed
  {
    public IdiLedColour Colour { get; set; }
    public IdiLedMode Mode { get; set; }
  }

  public abstract class ILedManager
  {
    public int LedCount
    {
      get { return _leds.Count; }
    }

    public abstract bool Initialize();
    public abstract Task<bool> InitializeAsync();

    public abstract void SetAllLeds(ref IdiLed idiLed);

    // Syncs Leds with the driver/hardware
    protected abstract bool Sync();
    protected abstract Task<bool> SyncAsync();

    private List<IdiLed> _leds = new List<IdiLed>();
  }
}
