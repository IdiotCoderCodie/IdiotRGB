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

    public IdiLedColour(byte v)
      : this(v, v, v, v)
    {
    }

    public byte Red;
    public byte Green;
    public byte Blue;
    public byte White;
  }
}
