using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdiotRGB
{
  interface ILedManager
  {
    int LedCount { get; }

    bool Initialize();
    Task<bool> InitializeAsync();

    void SetAllLeds(ref IdiLed idiLed);
  }
}
