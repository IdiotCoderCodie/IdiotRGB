using GigabyteRGBFusionSDK.Motherboard;
using GigabyteRGBFusionSDK.Peripherals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdiotRGB
{
  class RGBFusionManager : ILedManager
  {
    public RGBFusionManager()
    {
      _gLed = new GLedImpl();
      _gvLed = new GvLedImpl();
    }

    public override bool Initialize()
    {
      _gLed.InitAPI();

      int deviceCount = 0;
      int[] deviceArray = new int[128];
      _gvLed.Initialize(out deviceCount, deviceArray);

      return true;
    }

    public async override Task<bool> InitializeAsync() 
    {
      List<Task<uint>> taskList = new List<Task<uint>>();

      taskList.Add(Task.Run(() => _gLed.InitAPI()));

      int deviceCount = 0;
      int[] deviceArray = new int[128];
      taskList.Add(Task.Run(() => _gvLed.Initialize(out deviceCount, deviceArray)));

      uint[] results = await Task.WhenAll(taskList);

      bool passed = true;
      foreach (var res in results)
      {
        passed = (res == 0);
        if (!passed) break;
      }

      return true;
    }

    protected override bool Sync()
    {
      throw new NotImplementedException();
    }

    protected override Task<bool> SyncAsync()
    {
      throw new NotImplementedException();
    }

    private bool InitializeMobo()
    {

      return true;
    }

    private bool InitializePeriph()
    {
      return true;
    }

    private GLedImpl _gLed;
    private GvLedImpl _gvLed;
  }
}
