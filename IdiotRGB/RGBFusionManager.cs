using GigabyteRGBFusionSDK.Motherboard;
using GigabyteRGBFusionSDK.Peripherals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdiotRGB
{
  public class RGBFusionManager : ILedManager
  {
    public RGBFusionManager()
    {
      _gLed = new GLedImpl();
      _gvLed = new GvLedImpl();
      _gLedSettings = new List<GLedSetting>();
    }

    public override bool Initialize()
    {     
      return InitializeMobo() || InitializePeriph();
    }

    public async override Task<bool> InitializeAsync() 
    {
      List<Task<bool>> taskList = new List<Task<bool>>();

      taskList.Add(Task.Run(() => InitializeMobo()));
      taskList.Add(Task.Run(() => InitializePeriph()));

      bool[] results = await Task.WhenAll(taskList);

      bool didAnyPass = false;
      foreach (var res in results)
      {
        didAnyPass |= res;
      }

      if (!didAnyPass)
        return false;

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
      if (_gLed.InitAPI() != 0)
        return false;

      List<GLedType> ledTypes = new List<GLedType>();
      if(!_gLed.GetLedLayout(ledTypes))
      {
        return false;
      }

      // Create entry in _gLedSettings for all leds.
      for (int i = 0; i < ledTypes.Count; ++i)
      {
        _gLedSettings.Add(new GLedSetting());
      }

      // Start by turning all the Leds OFF.
      GLedSetting nullLed = new GLedSetting();
      nullLed.m_LedMode = GLedMode.Null;
      SetAllLeds(ref nullLed);
      
      return true;
    }

    private bool InitializePeriph()
    {
      int deviceCount = 0;
      int[] deviceArray = new int[128];

      if (_gvLed.Initialize(out deviceCount, deviceArray) != 0)
      {
        // throw ??
        return false;
      }

      // TODO: Start by turning all Leds OFF.

      return true;
    }

    // TODO: This should be private... public one should be inherited from ILedManager and take the IdiLed type.
    public void SetAllLeds(ref GLedSetting setting)
    {
      for (int i = 0; i < _gLedSettings.Count; ++i)
      {
        _gLedSettings[i] = setting;
      }
      ApplyLedSettings();
    }

    private void ApplyLedSettings()
    {
      uint res = _gLed.SetLedData(_gLedSettings.ToArray());
      res = _gLed.Apply(-1);
    }

    private GLedImpl _gLed;
    private GvLedImpl _gvLed;

    private List<GLedSetting> _gLedSettings;
    
  }
}
