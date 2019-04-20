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
      bool success = InitializeMobo();
      success |= InitializePeriph();
      return success;
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

    public override void SetAllLeds(ref IdiLed idiLed)
    {
      GvLedConfig toGvLed = IdiLedToGvLed(ref idiLed);
      GLedSetting toGLed = IdiLedToGLed(ref idiLed);

      SetAllPeriphLeds(ref toGvLed);
      SetAllMoboLeds(ref toGLed);
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
      SetAllMoboLeds(ref nullLed);
      
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

      GvLedConfig nullLedConfig = new GvLedConfig();
      nullLedConfig.on = false;
      SetAllPeriphLeds(ref nullLedConfig);

      return true;
    }

    private void SetAllMoboLeds(ref GLedSetting setting)
    {
      for (int i = 0; i < _gLedSettings.Count; ++i)
      {
        _gLedSettings[i] = setting;
      }
      ApplyLedSettings();
    }

    private void SetAllPeriphLeds(ref GvLedConfig ledConfig)
    {
      _gvLed.Save(-1, ledConfig);
    }

    private void ApplyLedSettings()
    {
      uint res = _gLed.SetLedData(_gLedSettings.ToArray());
      res = _gLed.Apply(-1);
    }

    private GvLedConfig IdiLedToGvLed(ref IdiLed idiLed)
    {
      GvLedConfig gvLedCfg = new GvLedConfig();
      gvLedCfg.color = new GvLedColour(idiLed.Colour.White, idiLed.Colour.Red, idiLed.Colour.Green, idiLed.Colour.Blue);

      switch(idiLed.Mode)
      {
        case IdiLedMode.STATIC:
          gvLedCfg.type = GvLedType.Consistent;
          break;
        case IdiLedMode.FLASH:
          gvLedCfg.type = GvLedType.SingleFlash;
          break;
        default:
          throw new NotImplementedException("Missing implementation of IdiLed mode " + idiLed.Mode.ToString() + " in RGBFusionManager.\n");
      }

      // TODO: Add these to IdiLed!
      gvLedCfg.maxBright = 10;
      gvLedCfg.minBright = 10;
      gvLedCfg.on = true; 

      return gvLedCfg;
    }

    private GLedSetting IdiLedToGLed(ref IdiLed idiLed)
    {
      GLedSetting gLedSetting = new GLedSetting();
      gLedSetting.m_Colour = new GLedColour(idiLed.Colour.White, idiLed.Colour.Red, idiLed.Colour.Green, idiLed.Colour.Blue);

      switch (idiLed.Mode)
      {
        case IdiLedMode.STATIC:
          gLedSetting.m_LedMode = GLedMode.Static;
          break;
        case IdiLedMode.FLASH:
          gLedSetting.m_LedMode = GLedMode.Flash;
          break;
        default:
          throw new NotImplementedException("Missing implementation of IdiLed mode " + idiLed.Mode.ToString() + " in RGBFusionManager.\n");
      }

      gLedSetting.m_MaxBrightness = 100;
      gLedSetting.m_MinBrightness = 100;

      return gLedSetting;
    }

    private GLedImpl _gLed;
    private GvLedImpl _gvLed;

    private List<GLedSetting> _gLedSettings;
    
  }
}
