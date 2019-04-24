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

    public int LedCount
    {
      get
      {
        return _gLedSettings.Count;
      }
    }

    public bool Initialize()
    {
      bool success = InitializeMobo();
      success |= InitializePeriph();
      return success;
    }

    public async Task<bool> InitializeAsync() 
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

    public void SetAllLeds(ref IdiLed idiLed)
    {
      SetAllPeriphLeds(ref idiLed);
      SetAllMoboLeds(ref idiLed);
    }

    public async Task SetAllLedsAsync(IdiLed idiLed)
    {
      List<Task> taskList = new List<Task>();
      taskList.Add(Task.Run(() => SetAllPeriphLeds(ref idiLed)));
      taskList.Add(Task.Run(() => SetAllMoboLeds(ref idiLed)));

      await Task.WhenAll(taskList);
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

    private void SetAllMoboLeds(ref IdiLed idiLed)
    {
      GLedSetting gledSetting = IdiLedToGLed(ref idiLed);
      SetAllMoboLeds(ref gledSetting);
    }

    private void SetAllMoboLeds(ref GLedSetting ledSetting)
    {
      for (int i = 0; i < _gLedSettings.Count; ++i)
      {
        _gLedSettings[i] = ledSetting;
      }
      ApplyGLedSettings();
    }

    private void SetAllPeriphLeds(ref IdiLed idiLed)
    {
      GvLedConfig ledConfig = IdiLedToGvLed(ref idiLed);
      SetAllPeriphLeds(ref ledConfig);
    }

    private void SetAllPeriphLeds(ref GvLedConfig ledConfig)
    {
      _gvLed.Save(-1, ledConfig);
    }

    private void ApplyGLedSettings()
    {
      uint res = _gLed.SetLedData(_gLedSettings.ToArray());
      res = _gLed.Apply(-1);
    }

    private GvLedConfig IdiLedToGvLed(ref IdiLed idiLed)
    {
      GvLedConfig gvLedCfg = new GvLedConfig();
      gvLedCfg.on = idiLed.Enabled;

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

      gvLedCfg.color = new GvLedColour(idiLed.Colour.White, idiLed.Colour.Red, idiLed.Colour.Green, idiLed.Colour.Blue);
      gvLedCfg.maxBright = idiLed.MaxBrightness / 10;
      gvLedCfg.minBright = idiLed.MinBrightness / 10;

      // Not sure what the format of these time values are... can't find information on it in samples of in the docs.
      gvLedCfg.time1 = idiLed.TimeMs0;
      gvLedCfg.time2 = (ushort)(idiLed.TimeMs0 + idiLed.TimeMs1);
      gvLedCfg.time3 = 255;
      gvLedCfg.speed = 1; // Does this solely affect the "flash" speed? 

      return gvLedCfg;
    }

    private GLedSetting IdiLedToGLed(ref IdiLed idiLed)
    {
      GLedSetting gLedSetting = new GLedSetting();
      gLedSetting.m_LedMode = GLedMode.Null;

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

      if (idiLed.Enabled)
        gLedSetting.m_Colour = new GLedColour(idiLed.Colour.White, idiLed.Colour.Red, idiLed.Colour.Green, idiLed.Colour.Blue);
      else
        gLedSetting.m_Colour = new GLedColour(0);

      gLedSetting.m_MinBrightness = idiLed.MinBrightness;
      gLedSetting.m_MaxBrightness = idiLed.MaxBrightness;

      // time0 == flash OFF time.
      // time1 =  time0 + flash ON time
      // time2 = 0 (infinite?)
      gLedSetting.m_Time0 = idiLed.TimeMs0;
      gLedSetting.m_Time1 = (ushort)(idiLed.TimeMs0 + idiLed.TimeMs1);
      gLedSetting.m_CtrlVal0 = 10;
      gLedSetting.m_Time2 = (ushort)(gLedSetting.m_CtrlVal0 * gLedSetting.m_Time1);
      

      return gLedSetting;
    }

    private GLedImpl _gLed;
    private GvLedImpl _gvLed;

    private List<GLedSetting> _gLedSettings;
    
  }
}
